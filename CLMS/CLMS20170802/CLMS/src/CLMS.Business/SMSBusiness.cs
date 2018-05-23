using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class SMSBusiness
    {

        private UdpClient _client;
        private IPEndPoint _remoteIpEndPoint;

        private static SMSBusiness _instance;

        public static SMSBusiness GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SMSBusiness();
            }
            return _instance;
        }

        private SMSBusiness()
        {
            _sendMessage = bool.Parse(ConfigManage.GetInstance().GetConfigByKey("SendMessage"));
            if (_sendMessage)
            {
                _client = new UdpClient();
                string ipAddress = ConfigManage.GetInstance().GetConfigByKey("PlatformAddress");
                int port = int.Parse(ConfigManage.GetInstance().GetConfigByKey("PlatformPort"));
                _remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                MonitorAlarmBusiness.GetInstance().SendMessage += SMSBusiness_SendMessage;
            }
        }

        private void SMSBusiness_SendMessage(AlarmModel alarm, string personIds)
        {
            var ids = personIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n.Trim()));
            var users = _telbookProvider.GetList("").Where(n => ids.Contains(n.Id));
            var phoneNumbers = users.Select(n => n.Telephone);
            if (alarm.State == AlarmStateType.OnAlarm)
            {
                Send2OPlatform(phoneNumbers, $"连通异常报警：{alarm.FirstTime.ToString("yyyy-MM-dd HH:mm:ss")}起{alarm.OrganName}使用的线路{alarm.LineName}连通异常，监控IP{alarm.IP}");
            }
            else if (alarm.State == AlarmStateType.Recover)
            {
                Send2OPlatform(phoneNumbers, $"连通恢复通知：{alarm.FirstTime.ToString("yyyy-MM-dd HH:mm:ss")}起{alarm.OrganName}使用的线路{alarm.LineName}连通已恢复，监控IP{alarm.IP}");
            }
        }

        private bool _sendMessage;

        private AlarmDataProvider _alertProvider;
        private TelbookDataProvider _telbookProvider;

        private void Send2OPlatform(IEnumerable<string> phoneNumbers, string message)
        {
             _client = new UdpClient();
            string users = string.Join(",", phoneNumbers);
            string sendText = users + "#" + message;
            var buffer = System.Text.Encoding.UTF8.GetBytes(sendText);
            _client.SendAsync(buffer, buffer.Length, _remoteIpEndPoint);
        }
    }
}
