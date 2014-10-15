using Newtonsoft.Json;

namespace NetBots.Bot.Interface
{
    public class Player
    {
        [JsonProperty("energy")]    public int Energy { get; set; }
        [JsonProperty("spawn")]     public int Spawn { get; set; }

        [JsonProperty("spawnDisabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool SpawnDisabled { get; set; }
    }
}
