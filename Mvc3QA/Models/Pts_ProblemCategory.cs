using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Pts_ProblemCategory
    {
        public int CategoryID { get; set; }
        public int ParentID { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public int ListOrder { get; set; }
    }
}
