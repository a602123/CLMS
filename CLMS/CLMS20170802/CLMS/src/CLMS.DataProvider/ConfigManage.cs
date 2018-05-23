using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace CLMS.DataProvider
{
    public class ConfigManage
    {
        private static ConfigManage _instance { set; get; }

        public static ConfigManage GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ConfigManage();
            }
            return _instance;
        }

        static IConfigurationRoot Configuration { get; set; }

        private ConfigManage()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            //Console.WriteLine($"option1 = {Configuration["subsection:suboption1"]}");
            _sysConnStr = Configuration["SysConnStr"];
        }

        private string _sysConnStr { get; set; }

        public string GetSysConnStr()
        {
            return _sysConnStr;
        }

        public string GetConfigByKey(string key)
        {
            return Configuration[key];
        }

        public void SetConfigByKey(string key, string value)
        {
            Configuration[key] = value;
        }
    }
}
