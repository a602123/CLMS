using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class PingCollector
    {
        /// <summary>
        /// Ping参数
        /// </summary>
        private PingParamter _param;
        /// <summary>
        /// Ping统计结果
        /// </summary>
        private PingState _state;
        /// <summary>
        /// Ping的具体结果
        /// </summary>
        public List<PingReply> _cache;
        /// <summary>
        /// 上次Ping的时间
        /// </summary>
        private DateTime _lastTime;
        /// <summary>
        /// 采集结束事件
        /// </summary>
        public event Action<int, PingState> Collected;

        public PingCollector(PingParamter param)
        {
            _param = param;
            _cache = new List<PingReply>();
            _lastTime = DateTime.MinValue;
            _inWork = false;
        }

        public PingState State { get { return _state; } }

        private bool _inWork;
        public void Begin()
        {
            if (!_inWork)
            {
                if ((DateTime.Now.AddSeconds(1) - _lastTime).Seconds > _param.Interval)
                {
                    _inWork = true;
                    DoPing();
                }
            }
        }

        private async Task DoPing()
        {
            byte[] buffer = Enumerable.Repeat((byte)1, _param.Pingsize).ToArray();
            var reply = await new Ping().SendPingAsync(_param.Ip, _param.Timeout, buffer);
            _cache.Add(reply);
            if (_cache.Count >= _param.Pingtimes)
            {
                CalcPing();
            }
            else
            {
                DoPing();
            }
        }

        private void CalcPing()
        {
            double lostRate = _cache.Count(n => n.Status != IPStatus.Success) / _cache.Count;
            double delay = _cache.Average(n => n.RoundtripTime);
            //bool state = lostRate > 0;
            bool state = lostRate == 0;
            var time = DateTime.Now;
            _lastTime = time;
            _state = new PingState()
            {
                Ip = _param.Ip,
                CollectTime = time,
                Delay = delay,
                LostRate = lostRate,
                State = state
            };
            _cache.Clear();
            //报警
            Collected?.Invoke(_param.Id, _state);
            _inWork = false;
        }


    }

    public class PingParamter
    {
        /// <summary>
        /// 线路Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 要Ping的Ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// Ping包的大小,默认32字节
        /// </summary>
        public int Pingsize { get; set; }
        /// <summary>
        /// 轮询ping的次数，默认4次
        /// </summary>
        public int Pingtimes { get; set; }
        /// <summary>
        /// 默认超时时间，2 秒
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 轮询周期，默认30秒
        /// </summary>
        public int Interval { get; set; }
    }

    public class PingState
    {
        /// <summary>
        /// Ping的Ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 是否通
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 丢包率（百分比显示，不用保留小数位）
        /// </summary>
        public double LostRate { get; set; }
        /// <summary>
        /// 平均延时（毫秒,显示时取整）
        /// </summary>
        public double Delay { get; set; }

        /// <summary>
        /// 本次数据采集时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        public string GetInfo()
        {
            return State ? $"{Ip}的丢包率为{LostRate.ToString("P")}、平均时延为{Delay.ToString("f2")}ms、本次采集时间为{CollectTime}" : $"{Ip}的线路发生线路没有连通,本次采集时间为{CollectTime}";
        }
    }
}