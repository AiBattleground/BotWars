using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NetBots.WebServer.Host.Startup))]
namespace NetBots.WebServer.Host
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
