using System;
using System.Collections.Generic;

namespace Mvc3QA.Models
{
    public class Kb_Articles
    {
        public int ArticleID { get; set; }
        public int CategoryID { get; set; }
        public string Title { get; set; }
        public string KeyWords { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> NumRead { get; set; }
        public Nullable<bool> IsRepost { get; set; }
        public int NumRatings { get; set; }
        public double RateValue { get; set; }
        public Nullable<int> ArticleStatus { get; set; }
        public string Source { get; set; }
        public string SourceUrl { get; set; }
        public Nullable<byte> ValueLevel { get; set; }
        public Nullable<byte> Difficulty { get; set; }
        public Nullable<int> RelateProblemID { get; set; }
        public string OriginAuthor { get; set; }
        public int CreateUserID { get; set; }
        public Nullable<int> NumComment { get; set; }
        public bool IsReviewed { get; set; }
        public string ReviewedBy { get; set; }
        public bool IsPublic { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }
        public string Note5 { get; set; }
        public virtual Kb_Categories Kb_Categories { get; set; }
    }
}
