using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc3QA.General
{
    /// <summary>
    /// 自定义视图路径
    /// </summary>
    public class BlogViewEngine  : RazorViewEngine
    {
        public BlogViewEngine(string key)
        {
            string path = string.IsNullOrWhiteSpace(key)||key=="default" ? "~/Views" : "~/Themes/" + key;

            ViewLocationFormats = new[] {
                path+"/{1}/{0}.cshtml",
                path+"/{1}/{0}.vbhtml",
                path+"/Shared/{0}.cshtml",
                path+"/Shared/{0}.vbhtml"
            };

            PartialViewLocationFormats = ViewLocationFormats;
        }
    }
}