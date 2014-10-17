
namespace NetBots.Core
{
    using Web;
   
    public class BotPlayer : Player
    {
        public string PlayerName { get; set; }
        public string Uri { get; set; }
        public char BotletId { get; set; }
        public Resource Resource { get; set; }
        public char deadBotletId { get; set; }
    }
}