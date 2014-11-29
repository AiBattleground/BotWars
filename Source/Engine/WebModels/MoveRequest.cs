using Newtonsoft.Json;

namespace NetBots.WebModels
{
    public class MoveRequest
    {
        [JsonProperty("state")]
        public GameState State { get; set; }

        [JsonProperty("player")]
        public string Player { get; set; }
    }
}
