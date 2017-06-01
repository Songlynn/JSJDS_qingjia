using System.Web.Mvc;

namespace qingjia_MVC.Areas.Password
{
    public class PasswordAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Password";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Password_default",
                "Password/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}