using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Pts_ProblemState
    {
        public Pts_ProblemState()
        {
            this.Pts_Problems = new List<Pts_Problems>();
            this.Pts_Records = new List<Pts_Records>();
        }

        public System.Guid ProblemStateID { get; set; }
        public string StateName { get; set; }
        public string Description { get; set; }
        public int ListOrder { get; set; }
        public bool IsFinalState { get; set; }
        public bool IsCustomerShow { get; set; }
        public Nullable<System.Guid> DeptID { get; set; }
        public Nullable<int> IsState { get; set; }
        public virtual ICollection<Pts_Problems> Pts_Problems { get; set; }
        public virtual ICollection<Pts_Records> Pts_Records { get; set; }
    }
}
