using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc3QA.Models
{
  
        /// <summary>
        /// 页面类型
        /// </summary>
        public class PageType
        {
            public int TypeId { get; set; }
            public string TypeName { get; set; }
        }

        /// <summary>
        /// 站点语言
        /// </summary>
        public class PageLang
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// 下拉项
        /// </summary>
        public class SelectItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// 转载网站信息实体类
        /// </summary>
        public class PublishWebModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Author { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
        }

        /// <summary>
        /// 文章标签
        /// </summary>
        public class TagInfo
        {
            public string Tag { get; set; }
            public int Count { get; set; }
        }
    }
