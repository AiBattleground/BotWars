using NetBots.Bot.Interface;

namespace NetBots.Core
{
    public class BotPlayer : Player
    {
        public string playerName { get; set; }
        public string color { get; set; }
        public char botletId { get; set; }
        public Resource resource { get; set; }
        public char deadBotletId { get; set; }
    }
}