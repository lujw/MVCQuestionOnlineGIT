using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using resWeb = Resource.Models.Web.Web;

namespace Mvc3QA.General
{
    /// <summary>
    /// 基本设置描述类
    /// </summary>
    [Serializable]
    public class GeneralConfigInfo : IConfigInfo
    {
        #region 字段
        private string webtitle = ""; //网站名称
        private string weburl = ""; //网站url地址
        private string webDescription = ""; //网站url地址
        private string icp = ""; //网站备案信息
        private string webpath = "/"; //网站路径
        private int indexPagerCount = 10;
        private int catePagerCount = 10;
        private int commentPagerCount = 10;
        private int notePagerCount = 10;
        private int decorateImgCount = 0;//插图总数
        private string thumbnailInfo = "";//缩略图配置
        private string theme = "";//模板风格
        private string defaultlang = "";//默认语言
        private string contributorCateIds = "";//转载时目标分类Id
        private string versionNo =  DateTime.Now.ToString("yyyyMMddhhmmss");//版本号(主要是js部分)
        private int maxSummaryCharCount = 0;//最大截取为summary的字符数
        private string adminEmail= "";
        private string smtpServer= "";
        private string smtpUser= "";
        private string smtpPass= "";
        private int smtpPort = 25;
        private int ifSendReplyEmail = 1;//是否发送回复邮件(1发送，2不发送)

        #endregion

        /// <summary>
        /// 网站名称
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(resWeb), Name = "Webtitle")]
        public string Webtitle
        {
            get { return webtitle; }
            set { webtitle = value; }
        }
        /// <summary>
        /// 网站描述
        /// </summary>
        [Display(ResourceType = typeof(resWeb), Name = "WebDescription")]
        public string WebDescription
        {
            get { return webDescription; }
            set { webDescription = value; }
        }

        /// <summary>
        /// 网站url地址
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Required_Msg")]
        [Display(ResourceType = typeof(resWeb), Name = "Weburl")]
        public string Weburl
        {
            get { return weburl; }
            set { weburl = value; }
        }

        /// <summary>
        /// 网站备案信息
        /// </summary>
        [Display(ResourceType = typeof(resWeb), Name = "Icp")]
        public string Icp
        {
            get { return icp; }
            set { icp = value; }
        }

         /// <summary>
         /// 站点路径
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "WebPath")]
         public string WebPath
         {
             get { return webpath; }
             set { webpath = value; }
         }

         /// <summary>
         /// 首页分页记录数
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "IndexPagerCount")]
         [RegularExpression(@"^\d+", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "MustNum")]
         public int IndexPagerCount
         {
             get { return indexPagerCount; }
             set { indexPagerCount = value; }
         }

         /// <summary>
         /// 文章列表分页记录数
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "CatePagerCount")]
         [RegularExpression(@"^\d+", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "MustNum")]
         public int CatePagerCount
         {
             get { return catePagerCount; }
             set { catePagerCount = value; }
         }
         /// <summary>
         /// 评论分页记录数
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "CommentPagerCount")]
         [RegularExpression(@"^\d+", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "MustNum")]
         public int CommentPagerCount
         {
             get { return commentPagerCount; }
             set { commentPagerCount = value; }
         }

         /// <summary>
         /// 留言分页记录数
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "NotePagerCount")]
         [RegularExpression(@"^\d+", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "MustNum")]
         public int NotePagerCount
         {
             get { return notePagerCount; }
             set { notePagerCount = value; }
         }

        /// <summary>
        /// 插图总数
        /// </summary>
         public int DecorateImgCount
         {
             get { return decorateImgCount; }
             set { decorateImgCount = value; }
         }
            
        /// <summary>
        /// 缩略图生成配置
        /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "ThumbnailInfo")]
         [RegularExpression(@"^[0-9x,]*$", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "ThumbnailFormat")]
         public string ThumbnailInfo
         {
             get { return thumbnailInfo; }
             set { thumbnailInfo = value; }
         }

         /// <summary>
         /// 版权
         /// </summary>
         public string CopyRight
         {
             get { return resWeb.CopyRight; }
         }

         /// <summary>
         /// 模板风格
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "Theme")]
         [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Reg_Msg")]
         public string Theme
         {
             get { return theme; }
             set { theme = value; }
         }

         /// <summary>
         /// 默认语言
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "DefaultLang")]
         [RegularExpression(@"^[a-zA-Z\-]+$", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Reg_Msg")]
         public string DefaultLang
         {
             get { return defaultlang; }
             set { defaultlang = value; }
         }

        /// <summary>
        /// web转载时目标分类Id集
        /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "ContributorCateIds")]
         [RegularExpression(@"^[0-9,]+$", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "Common_Reg_Msg")]
         public string ContributorCateIds
         {
             get { return contributorCateIds; }
             set { contributorCateIds = value; }
         }

        /// <summary>
        /// （js）版本号
        /// </summary>
         public string VersionNo
         {
             get { return versionNo; }
             set { versionNo = value; }
         }

         /// <summary>
         /// 截取为summary的最大字符数
         /// </summary>
         [Display(ResourceType = typeof(resWeb), Name = "MaxSummaryCount")]
         [RegularExpression(@"^\d+", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "MustNum")]
         public int MaxSummaryCharCount
         {
             get { return maxSummaryCharCount; }
             set { maxSummaryCharCount = value; }
         }

        [Display(ResourceType = typeof(resWeb), Name = "AdminEmail")]
         public string AdminEmail
         {
             get { return adminEmail; }
             set { adminEmail = value; }
         }

        [Display(ResourceType = typeof(resWeb), Name = "SmtpServer")]
         public string SmtpServer
         {
             get { return smtpServer; }
             set { smtpServer = value; }
         }

        [Display(ResourceType = typeof(resWeb), Name = "SmtpUser")]
         public string SmtpUser
         {
             get { return smtpUser; }
             set { smtpUser = value; }
         }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(resWeb), Name = "SmtpPass")]
         public string SmtpPass
         {
             get { return smtpPass; }
             set { smtpPass = value; }
         }

        [Display(ResourceType = typeof(resWeb), Name = "SmtpPort")]
        [RegularExpression(@"^\d+", ErrorMessageResourceType = typeof(resWeb), ErrorMessageResourceName = "MustNum")]
         public int SmtpPort
         {
             get { return smtpPort; }
             set { smtpPort = value; }
         }

        [Display(ResourceType = typeof(resWeb), Name = "IfSendReplyEmail")]
         public int IfSendReplyEmail
         {
             get { return ifSendReplyEmail; }
             set { ifSendReplyEmail = value; }
         }  
    }
}
