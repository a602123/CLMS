using CLMS.DataProvider;
using CLMS.Model;
using System;
using System.Collections.Generic;

namespace CLMS.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {

           
            //AlarmModel alarm = MonitorADOProvider.GetInstance().GetLastestAlarm(1113, AlarmType.LineNotConnected);
            //if (alarm == null)
            //{

            //    alarm = new AlarmModel()
            //    {
            //        AlarmCount = 1,
            //        Confirm = false,
            //        FirstTime = DateTime.Now,
            //        IP = line.LineIP,
            //        LineId = line.Id,
            //        LineName = line.Name,
            //        OrganId = line.OrganizationId,
            //        OrganName = line.OrganizationName,
            //        State = AlarmStateType.OnAlarm,
            //        LastTime = DateTime.Now,
            //        Type = AlarmType.LineNotConnected
            //    };
            //    int r1 = MonitorADOProvider.GetInstance().InsertAlarm(alarm);
            //}
            //else
            //{
            //    int r1 = MonitorADOProvider.GetInstance().UpdateAlarm(alarm.Id, alarm.AlarmCount + 1, DateTime.Now);
            //}

            //int r1 =MonitorADOProvider.GetInstance().RecoverAlarm(alarm.Id,DateTime.Now, AlarmStateType.Recover);
        }



        public void InsertTest()
        {
            //LineDataProvider _provider = new LineDataProvider();
            //double count = 0;
            //List<string> ipList = new IPHelper().GetIPListFromStartHost("192.167.1.1", 20, out count);
            //List<OrganModel> organList = new List<OrganModel>();
            //List<LineModel> lineList = new List<LineModel>();
            //double x = 0;
            //double y = 0;
            //int j = 0;
            //Random r = new Random();
            //for (int i = 0; i < 1000; i++)
            //{
            //    x = r.Next(-200, 200) * 0.1;
            //    y = r.Next(-200, 200) * 0.1;
            //    organList.Add(new OrganModel() { Name = "数据中心" + i, Description = i.ToString(), ParentId = 43, X = x.ToString(), Y = y.ToString() });
            //}
            //new OrganDataProvider().Insert(organList);

            //List<OrganModel> newOrganList = new OrganDataProvider().GetAllItems("", "", "");

            //foreach (var item in ipList)
            //{
            //    j = r.Next(43, 1000);
            //    lineList.Add(new LineModel() { AlarmMax = 3, Description = item, LineIP = item, OrganizationId = newOrganList[j].Id, LineType = LineType.mainline, PingInterval = 30, Pingsize = 32, Pingtimes = 4, ServiceProvider = ServiceProviderType.ChinaMobile, Timeout = 2, Name = item });
            //}
            //_provider.Insert(lineList);
        }
    }
}
