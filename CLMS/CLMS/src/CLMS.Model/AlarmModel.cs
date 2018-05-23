using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    public class AlarmModel
    {
        public int Id { get; set; }
        public int AlarmCount { get; set; }
        public string IP { get; set; }
        public string Note { get; set; }

        public DateTime FirstTime { get; set; }
        public DateTime LastTime { get; set; }
        public AlarmType Type { get; set; }        
        //public string TypeStr { get; set; }
        public bool Confirm { get; set; }
        public string LineName { get; set; }
        public int LineId { get; set; }

        public int OrganId { get; set; }
        public string OrganName { get; set; }

        public bool MessageSendState { get; set; }


        public AlarmStateType State { get; set; }
        public DateTime? RecoverDate { get; set; }

        //public bool LineState { get; set; }

        public string SMSTelphone { get; set; }
    }
}
