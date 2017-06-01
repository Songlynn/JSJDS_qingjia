using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(qingjia_MVC.Startup))]
namespace qingjia_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
