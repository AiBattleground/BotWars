using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using NetBots.Web;
using Newtonsoft.Json;

namespace NetBots.WebServer.Host.Hubs
{
    public class WarViewHub : Hub
    {
        public void Send()
        {
            Clients.All.sendLatestMove();
        }
    }

    
    public class WarViewModel
    {
        [JsonProperty("p1Name")]
        public string P1Name { get; set; }
        [JsonProperty("p2Name")]
        public string P2Name { get; set; }
        [JsonProperty("state")]
        public GameState State { get; set; }
        [JsonProperty("alert")]
        public string Alert { get; set; }
    }
}