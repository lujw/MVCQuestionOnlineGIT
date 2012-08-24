using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc3QA.Models;

using Common;

namespace Mvc3QA.General
{
    /// <summary>
    /// 全局过滤器
    /// </summary>
    public class BlogActionAttributeFilter: ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{ //在Action执行前执行

        //}

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{ //在Action执行之后执行
        //}

        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{ //在Result执行之后 
        //    base.OnResultExecuted(filterContext);
        //}

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        { //在Result执行之前
            base.OnResultExecuting(filterContext);
            if (filterContext.Result is ViewResult)
            {
                filterContext.Controller.ViewBag.CI = Mvc3QA.General.WebUtils.configinfo;
            }
        }
    }

    /// <summary>
    /// 前台页面过滤器
    /// </summary>
    public class WebFilter : ActionFilterAttribute
    {
        //[Inject]
        //public IServices myService { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                //base.OnActionExecuted(filterContext);
                //if (filterContext.Result is ViewResult)
                //{
                //    List<CategoryModel> lst = myService.getFCategoryList("8", "");

                //    foreach (CategoryModel category in lst)
                //    {
                //        var re = myService.GetArticles(0, Utils.StrToInt(category.CateId), 0);
                //        if (re.Count() > 0)
                //        {
                //            foreach (blog_varticle varticle in re)
                //            {
                //                filterContext.Controller.ViewData["G_" + varticle.id.ToString()] = varticle;
                //            }
                //        }
                //    }
                //}
            }
            catch { }
        }
    }
}