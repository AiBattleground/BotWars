using System;
using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(GameHost.App_Start.SignalRStartup))]
namespace GameHost.App_Start
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}