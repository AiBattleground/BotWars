using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace NetBotsHostProject.Hubs
{
    public class WarViewHub : Hub
    {
        public void Send()
        {
            Clients.All.sendLatestMove();
        }
    }
}