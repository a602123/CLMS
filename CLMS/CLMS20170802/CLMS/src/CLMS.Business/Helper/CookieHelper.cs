using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using Newtonsoft.Json;

namespace CLMS.Business
{
    public class CookieHelper
    {
        private static Dictionary<string, CookieHelper> _instancePool = new Dictionary<string, CookieHelper>();

        private static object _lock = new object();
        public static CookieHelper GetInstance(string root)
        {
            lock (_lock)
            {
                if (!_instancePool.ContainsKey(root))
                {
                    _instancePool.Add(root, new CookieHelper(root));
                }
            }
            return _instancePool[root];
        }

        private CookieHelper(string root)
        {
            _root = root;
        }
        private string _root;

        public T GetCookie<T>( HttpRequest request)
        {
            string cook = request.Cookies[_root];
            if (cook == null)
            {
                return default(T);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(WebUtility.HtmlDecode(cook));
            }
            catch (Exception ex)
            {
                LogWriter.LogError(string.Format("读取Cookie[{0}]时发生错误:{2}" + _root, ex.Message));
                throw new Exception("读取Cookie信息出现错误");
            }
        }

        public void CreateCookies<T>(T model, HttpResponse response)
        {
            try
            {

                response.Cookies.Delete(_root);
                string cookie = WebUtility.HtmlEncode(JsonConvert.SerializeObject(model));
                response.Cookies.Append(_root, cookie);
            }
            catch (Exception ex)
            {
                LogWriter.LogError("保存Cookie时发生错误:" + ex.Message);
                throw new Exception("保存Cookie信息出现错误");
            }

        }

        public void CleanCookie(HttpResponse response)
        {
            response.Cookies.Delete(_root);
        }
    }
}
