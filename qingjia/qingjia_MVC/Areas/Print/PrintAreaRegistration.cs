using System.Web.Mvc;

namespace qingjia_MVC.Areas.Print
{
    public class PrintAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Print";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Print_default",
                "Print/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}