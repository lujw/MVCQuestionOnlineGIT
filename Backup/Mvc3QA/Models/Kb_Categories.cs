using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Kb_Categories
    {
        public Kb_Categories()
        {
            this.Kb_Articles = new List<Kb_Articles>();
        }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int ParentID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int ArticleCount { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public string Path { get; set; }
        public string AllowedUserGroups { get; set; }
        public bool IsPublic { get; set; }
        public bool RequireReview { get; set; }
        public int ListOrder { get; set; }
        public virtual ICollection<Kb_Articles> Kb_Articles { get; set; }
    }
}
