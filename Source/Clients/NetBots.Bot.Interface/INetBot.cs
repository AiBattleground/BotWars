using System.Collections.Generic;

namespace NetBots.Bot.Interface
{
    public interface INetBot
    {
        string Name { get; }
        string Color { get; }

        IEnumerable<BotletMove> GetMoves(MoveRequest request);
    }
}
