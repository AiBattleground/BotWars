using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.SignalR;
using NetBots.GameEngine;
using NetBots.Web;
using NetBotsHostProject.Hubs;
using Newtonsoft.Json;

namespace NetBots.WebServer.Host.Hubs
{
    public class WarViewHub : Hub
    {
        public static void SendGameState(WarViewModel model, string userId)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<WarViewHub>();
            hub.Clients.User(userId).Send(model);
        }

        public static void BroadcastGameState(WarViewModel model)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<WarViewHub>();
            hub.Clients.All.Broadcast(model);
        }

        public async Task<string> StartGame(int bot1Id, int bot2Id)
        {
            var userId = Context.ConnectionId;
            var gameManager = new WarGameManager();
            var winner = await gameManager.StartGame(bot1Id, bot2Id, userId);
            return winner;
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