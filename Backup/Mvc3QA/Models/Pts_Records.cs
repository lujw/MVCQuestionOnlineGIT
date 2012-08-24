using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Pts_Records
    {
        public Pts_Records()
        {
            this.Pts_ProblemStateRecord = new List<Pts_ProblemStateRecord>();
        }

        public System.Guid RecordID { get; set; }
        public System.Guid ProblemID { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string Content { get; set; }
        public System.Guid ProblemStateID { get; set; }
        public Nullable<int> RecordClass { get; set; }
        public System.Guid SrcStateID { get; set; }
        public string SrcUserID { get; set; }
        public int AssignType { get; set; }
        public System.Guid AssignStateID { get; set; }
        public string AssignToObjectID { get; set; }
        public string AssignTo { get; set; }
        public Nullable<int> ResponseWorkTime { get; set; }
        public string ResponseTotalTime { get; set; }
        public Nullable<int> RealTimeCost { get; set; }
        public string Course { get; set; }
        public string Describe { get; set; }
        public int ListOrder { get; set; }
        public Nullable<System.DateTime> YFTime { get; set; }
        public Nullable<int> YFWay { get; set; }
        public virtual Pts_Problems Pts_Problems { get; set; }
        public virtual Pts_ProblemState Pts_ProblemState { get; set; }
        public virtual ICollection<Pts_ProblemStateRecord> Pts_ProblemStateRecord { get; set; }
    }
}
