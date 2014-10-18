using System;
using Owin;
using Microsoft.Owin;

//[assembly: OwinStartup(typeof(NetBotsHostProject.SignalRStartup))]
namespace NetBotsHostProject
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}