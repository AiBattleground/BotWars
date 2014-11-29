using System.Collections.Generic;
using NetBots.WebModels;

namespace NetBots.GameEngine
{
    public class PlayerMoves
    {
        public string PlayerName { get; set; }
        public IEnumerable<BotletMove> Moves { get; set; }
    }
}
