using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetBots.Bot.Interface
{
    public class GameState
    {
        public int gameId { get; set; }
        public int turnId { get; set; }
        public string apiKey { get; set; }
        public string secretKey { get; set; }
        public int rows { get; set; }
        public int cols { get; set; }
        public Player p1 { get; set; }
        public Player p2 { get; set; }
        public string grid { get; set; }
        public int maxTurns { get; set; }
        public int turnsElapsed { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string winner { get; set; }
    }
}
