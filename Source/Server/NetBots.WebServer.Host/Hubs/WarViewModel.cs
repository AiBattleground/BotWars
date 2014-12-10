using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBots.WebModels;
using Newtonsoft.Json;

namespace NetBots.WebServer.Host.Hubs
{
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