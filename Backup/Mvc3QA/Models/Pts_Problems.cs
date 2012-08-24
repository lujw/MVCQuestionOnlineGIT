using System;
using System.Collections.Generic;
using res = Resource.Models.Problems.Problems;
using resWeb = Resource.Models.Web.Web;
using System.ComponentModel.DataAnnotations;

namespace Mvc3QA.Models
{
    public class Pts_Problems
    {
        public Pts_Problems()
        {
            this.Pts_ProblemHistory = new List<Pts_ProblemHistory>();
            this.Pts_Records = new List<Pts_Records>();
        }
        public System.Guid ProblemID { get; set; }
        [Display(ResourceType = typeof(res), Name = "CategoryID")]
        public int CategoryID { get; set; }
      [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(res), Name = "Title")]
        public string Title { get; set; }        
        [Display(ResourceType = typeof(res), Name = "Description")]
        public string Description { get; set; }
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(res), Name = "Content")]
        public string Content { get; set; }
        [Display(ResourceType = typeof(res), Name = "AssignedTo")]
        public string AssignedTo { get; set; }
        public string CreatUserName { get; set; }
        public string AssignedToUser { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(res), Name = "StartTime")]
        public System.DateTime StartTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(res), Name = "CloseTime")]
        public System.DateTime CloseTime { get; set; }
        public String HandlingUser { get; set; }
        public Guid ProblemStateID { get; set; }
        public Nullable<System.DateTime> RealStartTime { get; set; }
        public Nullable<System.DateTime> RealEndTime { get; set; }
        public bool IsClosed { get; set; }
        public Nullable<bool> IsStart { get; set; }
        public string SolveUser { get; set; }
        //public virtual Pts_ProblemState Pts_ProblemState { get; set; }
        public virtual ICollection<Pts_ProblemHistory> Pts_ProblemHistory { get; set; }
        public virtual ICollection<Pts_Records> Pts_Records { get; set; }
    }
}
