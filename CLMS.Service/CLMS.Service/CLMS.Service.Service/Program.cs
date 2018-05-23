using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Net.Http.Formatting;
using System.IO;
using System.Windows.Forms;

namespace CLMS.Service.Service
{
    static class Program
    {
        static Program()
        {
            //AppDomain.CurrentDomain.UnhandledException+=
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
#if DEBUG
            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(ConfigurationManager.AppSettings["ServiceHost"]);

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("datatype", "json", "application/json"));

            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Application.Run(new Form1());
            }
#else
             ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
