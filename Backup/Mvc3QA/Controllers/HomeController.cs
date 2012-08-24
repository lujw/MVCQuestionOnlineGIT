using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc3QA.Models;

namespace Mvc3QA.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            ViewBag.Message = "欢迎使用 ASP.NET MVC!";

            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
