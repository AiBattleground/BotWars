using Newtonsoft.Json;

namespace NetBots.Bot.Interface
{
    public class Player
    {
        public int energy { get; set; }
        public int spawn { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool spawnDisabled { get; set; }
    }
}
