using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CLMS.Service.Business
{
    public class MonitorCenter
    {
        private DateTime _lastTime;

        private double _interval;

        private static Object _instance;

        Object _provider;

        public static Object GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Object();
            }
            return _instance;
        }

        private Timer _timer;

        public MonitorCenter()
        {
            _timer = new Timer();
            _timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            _timer.Elapsed += Timer_Elapsed;
            _provider = null;//new MonitorProvider();
            Refresh();
        }

        public void Refresh()
        {
            Tuple<DateTime, int> param = null;
            _lastTime = param.Item1;
            _interval = 3600 * 1000 * param.Item2;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            if ((now.AddMinutes(1)-_lastTime).TotalMilliseconds >_interval)
            {
                //MonitorBusiness.GetInstance().GetAllIP();
                _lastTime = DateTime.Now;
                //更新上次修改的时间
            }
        }

        public void Start()
        {
            if (!_timer.Enabled)
            {
                _timer.Start();
            }
        }


        public void Stop()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
            }
        } 
    }
}
