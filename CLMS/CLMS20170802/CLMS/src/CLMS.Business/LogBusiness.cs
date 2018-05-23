using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLMS.DataProvider;
using CLMS.Model;

namespace CLMS.Business
{
    public class LogBusiness
    {
        private LogDataProvider _provider;

        private static LogBusiness _instance;
        public static LogBusiness GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LogBusiness();
            }
            return _instance;
        }

        public LogBusiness()
        {
            _provider = new LogDataProvider();
        }

        public void Add(LogModel log)
        {
            _provider.Insert(log);
        }

        public PageableData<LogModel> GetPage(int? logType, DateTime? startTime, DateTime? endTime, int limit, int offset)
        {
            string conditionTimeStr = "";
            if (startTime != null)
            {
                conditionTimeStr += " AND TIME >=@StartTime";
            }
            if (endTime != null)
            {
                conditionTimeStr += " AND TIME <=@EndTime";
            }
            if (conditionTimeStr.Length > 0)
            {
                conditionTimeStr = conditionTimeStr.Substring(4);
            }
            //var condition = new ConditionHelper().And("Type", logType, CompareType.Equal)
            //                                        .And(conditionTimeStr)
            //                                         .ToString();
            string condition = string.Empty;
            if (logType>-1)
            {
                condition=new ConditionHelper().And("Type", logType, CompareType.Equal)
                                                    .And(conditionTimeStr)
                                                    .ToString();
            }
            condition += new ConditionHelper().And(conditionTimeStr).ToString();
            var searchObj = new
            {
                Type = logType,
                StartTime = startTime,
                EndTime = endTime
            };
            return _provider.GetPage(condition, searchObj, " ORDER BY TIME DESC", offset, limit);
        }

        public IEnumerable<LogModel> GetTop10()
        {
            return _provider.GetList(" ", null, " order by Time desc LIMIT 0,10");
        }
    }
}
