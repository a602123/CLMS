using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class LogHelper
    {
        private string LOG = "{0} {1}\r\n";

        private string INFO = "事件内容:{0}";

        private string ERROR = "错误内容:{0}";

        private string _logerSource;

        private string _logPath
        {
            get
            {
                string str = Directory.GetCurrentDirectory();
                return str + "\\Logs\\" + this._logerSource + "\\";
            }
        }

        public LogHelper(string logerSource)
        {
            this._logerSource = logerSource;
        }

        public void LogInfo(string info)
        {
            try
            {
                string log = string.Format(this.INFO, info);
                this.WriteString("InfoLog", log);
            }
            catch
            {
            }
        }

        public void LogError(string error)
        {
            try
            {
                string log = string.Format(this.ERROR, error);
                this.WriteString("ErrorLog", log);
            }
            catch
            {
            }
        }

        private bool WriteString(string logType, string log)
        {
            bool flag = false;
            string text = this._logPath + logType;
            DirectoryInfo directoryInfo = new DirectoryInfo(text);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            bool result;
            string path = text + "\\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".log";
            using (FileStream fs = new FileStream(path,FileMode.Append))
            {
                using (StreamWriter streamWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    string arg = DateTime.Now.ToString("HH:mm:ss");
                    streamWriter.WriteLine(string.Format(this.LOG, arg, log));
                    streamWriter.Flush();
                    result = flag;
                }
            }
            
            return result;
        }
    }
}
