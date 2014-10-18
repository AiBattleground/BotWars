using Newtonsoft.Json;

namespace NetBots.Web
{
    public class BotletMove
    {
        public BotletMove() { }

        public BotletMove(int from, int to)
        {
            this.From = from;
            this.To = to;
        }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }
    }
}