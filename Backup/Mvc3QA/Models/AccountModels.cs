using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using res = Resource.Models.Account.Account;
using resWeb = Resource.Models.Web.Web;
namespace Mvc3QA.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "电子邮件地址")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
    public class UserProfileModel
    {
        [Display(ResourceType = typeof(res), Name = "NickName")]
        public string NickName { get; set; }
        [Display(ResourceType = typeof(res), Name = "Signature")]
        public string Signature { get; set; }
        [Display(ResourceType = typeof(res), Name = "Intro")]
        public string Intro { get; set; }
        [Display(ResourceType = typeof(res), Name = "Gender")]
        public string Gender { get; set; }
        [Display(ResourceType = typeof(res), Name = "Department")]
        public string Department { get; set; }
        [Display(ResourceType = typeof(res), Name = "Birth")]
        public string Birth { get; set; }
        [Display(ResourceType = typeof(res), Name = "Location")]
        public string Location { get; set; }
        [Display(ResourceType = typeof(res), Name = "Website")]
        public string Website { get; set; }
        [Display(ResourceType = typeof(res), Name = "QQ")]
        public string QQ { get; set; }
        [Display(ResourceType = typeof(res), Name = "Sina")]
        public string Sina { get; set; }
        [Display(ResourceType = typeof(res), Name = "Facebook")]
        public string Facebook { get; set; }
        [Display(ResourceType = typeof(res), Name = "Twitter")]
        public string Twitter { get; set; }
        [Display(ResourceType = typeof(res), Name = "Medals")]
        public string Medals { get; set; }
        [Display(ResourceType = typeof(res), Name = "Phone")]
        public string Phone { get; set; }
        [Display(ResourceType = typeof(res), Name = "EmailNotice")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Reg_Msg")]
        public string Email { get; set; }
        [Display(ResourceType = typeof(res), Name = "IsSendEmail")]
        public string IsSendEmail { get; set; }
    }
}
