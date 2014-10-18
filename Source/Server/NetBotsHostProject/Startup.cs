using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NetBotsHostProject.Startup))]
namespace NetBotsHostProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
