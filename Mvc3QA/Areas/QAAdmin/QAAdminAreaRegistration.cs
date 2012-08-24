using System.Web.Mvc;

namespace Mvc3QA.Areas.QAAdmin
{
    public class QAAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QAAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QAAdmin_default",
                "QAAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
