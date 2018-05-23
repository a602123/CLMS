using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CLMS.Business
{
    /// <summary>
    /// 在适当的地方初始化
    /// </summary>
    public class MonitorCenter
    {
        /// <summary>
        /// int 是线路的Id
        /// </summary>
        private Dictionary<int, PingCollector> _collectors;

        private static MonitorCenter _instance;
        /// <summary>
        /// 上报事件(报警处捆绑)
        /// </summary>
        public event Action<int, PingState> Report;

        private void OnCollected(int id, PingState state)
        {
            Report?.Invoke(id, state);
        }

        public static MonitorCenter GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MonitorCenter();
            }
            return _instance;
        }
        private Timer _timer;

        public MonitorCenter()
        {
            _collectors = new Dictionary<int, PingCollector>();
            _inWork = false;
        }

        private object _lock = new object();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="lines"></param>
        public void Init(IEnumerable<LineModel> lines)
        {
            //Report += MonitorAlarmBusiness.GetInstance().WriteAlarmToDB;
            Report += MonitorStateBusiness.GetInstance().UpdateStateToDB;

            lock (_lock)
            {
                foreach (var collector in _collectors)
                {
                    collector.Value.Collected -= OnCollected;
                }
                _collectors.Clear();
                Add(lines);
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="lines"></param>
        public void Add(IEnumerable<LineModel> lines)
        {
            lock (_lock)
            {
                foreach (var line in lines)
                {
                    if (_collectors.ContainsKey(line.Id))
                    {
                        _collectors[line.Id].Collected -= OnCollected;

                        _collectors[line.Id] = new PingCollector(new PingParamter()
                        {
                            Id = line.Id,
                            Interval = line.PingInterval,
                            Ip = line.LineIP,
                            Pingsize = line.Pingsize,
                            Pingtimes = line.Pingtimes,
                            Timeout = line.Timeout
                        });
                        _collectors[line.Id].Collected += OnCollected;
                    }
                    else
                    {
                        _collectors.Add(line.Id, new PingCollector(new PingParamter()
                        {
                            Id = line.Id,
                            Interval = line.PingInterval,
                            Ip = line.LineIP,
                            Pingsize = line.Pingsize,
                            Pingtimes = line.Pingtimes,
                            Timeout = line.Timeout
                        }));
                        _collectors[line.Id].Collected += OnCollected;
                    }

                }
            }
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="ids"></param>
        public void Remove(IEnumerable<int> ids)
        {
            lock (_lock)
            {
                foreach (var id in ids)
                {
                    if (_collectors.ContainsKey(id))
                    {
                        _collectors[id].Collected -= OnCollected;
                        _collectors.Remove(id);
                    }
                }
            }
        }
        /// <summary>
        /// 通过Id获取状态（我更推荐在Report回掉函数里把结果入库，然后通过数据库查找，可以给line那个表增加一列用来存放json序列化好的State，如果需要一些数据，也可单独增加列）
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Dictionary<int, PingState> GetState(IEnumerable<int> ids)
        {
            Dictionary<int, PingState> result = new Dictionary<int, PingState>();
            foreach (var id in ids)
            {
                result.Add(id, _collectors[id].State);
            }
            return result;
        }

        public void Start()
        {
            if (_timer == null)
            {
                _timer = new Timer(Timer_Elapsed, null, 5000, (int)TimeSpan.FromSeconds(5).TotalMilliseconds);
                _lastTime = DateTime.Now;
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
        private bool _inWork;
        private DateTime _lastTime;
        

        private void Timer_Elapsed(object state)
        {
            if (!_inWork)
            {
                _inWork = true;

                foreach (var collector in _collectors)
                {
                    collector.Value.Begin();
                }
                if ((DateTime.Now - _lastTime) > TimeSpan.FromMinutes(1))
                {
                    GC.Collect();
                    _lastTime = DateTime.Now;
                }
                _inWork = false;
            }

        }



    }
}
