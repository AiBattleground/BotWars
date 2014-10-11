
namespace NetBots.Bot.Interface
{
    public class BotletMove
    {
        public BotletMove() { }

        public BotletMove(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public int from { get; set; }
        public int to { get; set; }
    }
}