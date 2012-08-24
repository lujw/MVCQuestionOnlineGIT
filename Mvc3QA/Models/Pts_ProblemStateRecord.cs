using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Pts_ProblemStateRecord
    {
        public System.Guid RecordHistoryID { get; set; }
        public System.Guid RecordID { get; set; }
        public System.Guid StateID { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime MaxEndTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public int KeepTime { get; set; }
        public Nullable<int> WorkgroupID { get; set; }
        public string UserID { get; set; }
        public Nullable<int> BizKeepTime { get; set; }
        public virtual Pts_Records Pts_Records { get; set; }
    }
}
