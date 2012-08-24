using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Pts_ProblemHistory
    {
        public System.Guid ProblemHistoryID { get; set; }
        public System.Guid ProblemID { get; set; }
        public System.DateTime ChangeDate { get; set; }
        public string UserID { get; set; }
        public string Content { get; set; }
        public virtual Pts_Problems Pts_Problems { get; set; }
    }
}
