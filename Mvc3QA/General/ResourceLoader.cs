using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc3QA.General
{
    /// <summary>
    /// 动态切换语言,这个不用了
    /// Mvc3QA.General.ResourceLoader.SetCurrentThreadCulture(Session);
    /// </summary>
    public static class ResourceLoader
    {
        public static void SetCurrentThreadCulture(HttpSessionStateBase session)
        {
            if (session != null && session["Culture"] != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = (System.Globalization.CultureInfo)session["Culture"];
                System.Threading.Thread.CurrentThread.CurrentUICulture = (System.Globalization.CultureInfo)session["Culture"];
            }
        }
    }
}