using Newtonsoft.Json;

namespace NetBots.Web
{
    public class GameState
    {
        [JsonProperty("gameId")]        public string GameId { get; set; }
        [JsonProperty("turnId")]        public int TurnId { get; set; }
        [JsonProperty("apiKey")]        public string ApiKey { get; set; }
        [JsonProperty("secretKey")]     public string SecretKey { get; set; }
        [JsonProperty("rows")]          public int Rows { get; set; }
        [JsonProperty("cols")]          public int Cols { get; set; }
        [JsonProperty("p1")]            public Player P1 { get; set; }
        [JsonProperty("p2")]            public Player P2 { get; set; }
        [JsonProperty("grid")]          public string Grid { get; set; }
        [JsonProperty("maxTurns")]      public int MaxTurns { get; set; }
        [JsonProperty("turnsElapsed")]  public int TurnsElapsed { get; set; }

        [JsonProperty("winner", NullValueHandling = NullValueHandling.Ignore)]
        public string Winner { get; set; }
    }
}
