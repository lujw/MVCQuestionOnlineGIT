using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using res = Resource.Models.Category.Category;
using resWeb = Resource.Models.Web.Web;
namespace Mvc3QA.Models
{
    /// <summary>
    /// 分类实体类
    /// </summary>
    public class CategoryModel
    {
        //分类id
        public string CateId { get; set; }
        //分类名称
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(res), Name = "CateName")]
        public string CateName { get; set; }
        //页面类型
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(res), Name = "Type")]
        public string Type { get; set; }
        //首页调用时候显示记录条数，IsList为1时有效
        [Display(ResourceType = typeof(res), Name = "ListNum")]
        [RegularExpression(@"^\d+", ErrorMessageResourceType = typeof(res), ErrorMessageResourceName = "ListNum_Reg_Msg")]
        public string ListNum { get; set; }
        //排序id
        public string OrderId { get; set; }
        //该分类是否可评论
        [Display(ResourceType = typeof(res), Name = "ReplyPermit")]
        public string ReplyPermit { get; set; }
        //父分类id
        [Display(ResourceType = typeof(res), Name = "ParentId")]
        public string ParentId { get; set; }
        //是否在站点导航里显示
        [Display(ResourceType = typeof(res), Name = "IsNav")]
        public string IsNav { get; set; }
        //子分类数
        public string SubCount { get; set; }
        //是否显示在首页
        [Display(ResourceType = typeof(res), Name = "IsIndex")]
        public string IsIndex { get; set; }
        //状态
        [Display(ResourceType = typeof(res), Name = "Status")]
        public string Status { get; set; }
        //路径
        public string Path { get; set; }
        //别名
        [Display(ResourceType = typeof(res), Name = "ReName")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z0-9_\-]{2,19}$", ErrorMessageResourceType = typeof(res), ErrorMessageResourceName = "ReName_Reg_Msg")]
        public string ReName { get; set; }
        //自定义视图
        [Display(ResourceType = typeof(res), Name = "CustomView")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z0-9_\-]{2,19}$", ErrorMessageResourceType = typeof(res), ErrorMessageResourceName = "ReName_Reg_Msg")]
        public string CustomView { get; set; }

        //页面类型有，普通单页，文章列表，留言板，论坛，投稿。
    }

    /// <summary>
    /// 分类语言信息类
    /// </summary>
    public class CategoryLangModel
    {
        //分类id
        public string CateId { get; set; }
        //分类名称
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(res), Name = "CateName")]
        public string CateName { get; set; }
        //自定义视图
        [Display(ResourceType = typeof(res), Name = "CustomView")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z0-9_\-]{2,19}$", ErrorMessageResourceType = typeof(res), ErrorMessageResourceName = "ReName_Reg_Msg")]
        public string CustomView { get; set; }
    }
}