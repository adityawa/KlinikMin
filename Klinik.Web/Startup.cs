using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Klinik.Web.Startup))]

namespace Klinik.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
