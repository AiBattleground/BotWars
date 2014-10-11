
using NetBots.Bot.Interface;

namespace NetBots.Core
{
    public class MoveRequest
    {
        public GameState state { get; set; }
        public string player { get; set; }
    }
}
