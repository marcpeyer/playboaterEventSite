using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EventSiteWeb.Startup))]
namespace EventSiteWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
