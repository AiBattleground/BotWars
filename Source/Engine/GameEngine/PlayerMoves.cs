using System.Collections.Generic;

namespace NetBots.GameEngine
{
    using Web;

    public class PlayerMoves
    {
        public string PlayerName { get; set; }
        public IEnumerable<BotletMove> Moves { get; set; }
    }
}
