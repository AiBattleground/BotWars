using System;
using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(NetBots.WebServer.Host.App_Start.SignalRStartup))]
namespace NetBots.WebServer.Host.App_Start
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}