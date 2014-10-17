using Newtonsoft.Json;

namespace NetBots.Web
{
    public class MoveRequest
    {
        [JsonProperty("state")]
        public GameState State { get; set; }

        [JsonProperty("player")]
        public string Player { get; set; }
    }
}
