using System.Web.Mvc;

namespace qingjia_MVC.Areas.AddressList
{
    public class AddressListAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AddressList";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AddressList_default",
                "AddressList/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}