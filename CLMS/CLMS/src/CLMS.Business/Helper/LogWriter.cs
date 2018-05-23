using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class LogWriter
    {
        private static LogHelper _log;
        private static LogHelper GetLog()
        {
            string logerSource = "Business";

            LogHelper log = new LogHelper(logerSource);

            return log;
        }

        public static void LogError(string message)
        {
            if (_log == null)
            {
                _log = GetLog();
            }
            _log.LogError(message);
        }

        public static void LogInfo(string message)
        {
            if (_log == null)
            {
                _log = GetLog();
            }
            _log.LogInfo(message);
        }

    }
}
