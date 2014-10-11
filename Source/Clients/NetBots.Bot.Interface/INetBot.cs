using System.Collections.Generic;

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
