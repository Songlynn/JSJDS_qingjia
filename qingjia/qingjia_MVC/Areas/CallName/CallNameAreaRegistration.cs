using System.Web.Mvc;

namespace qingjia_MVC.Areas.CallName
{
    public class CallNameAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CallName";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CallName_default",
                "CallName/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}