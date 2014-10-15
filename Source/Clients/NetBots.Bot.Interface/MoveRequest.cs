using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace NetBots.Bot.Interface
{
    public class MoveRequest
    {
        [JsonProperty("state")]
        public GameState State { get; set; }

        [JsonProperty("player")]
        public string Player { get; set; }
    }
}
