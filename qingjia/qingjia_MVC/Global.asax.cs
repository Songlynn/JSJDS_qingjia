using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FineUIMvc;
using Newtonsoft.Json.Linq;

namespace qingjia_MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(JArray), new JArrayModelBinder());
            ModelBinders.Binders.Add(typeof(JObject), new JObjectModelBinder());
        }
    }
}
