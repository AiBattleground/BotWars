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
        ICollection<BotletMove> Moves { get; }
        string Color { get; }
    }
}
