using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NetBots.Bot.Interface
{
    public interface INetBot
    {
        string Name { get; }

        IBotLetMoveCollection GetMoves(GameState gameState);
    }

    public interface IBotLetMoveCollection
    {
        IEnumerable<BotletMove> Moves { get; }
        string Color { get; }
    }
}
