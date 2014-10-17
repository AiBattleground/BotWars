using System.Collections.Generic;

namespace NetBots.Bot.Interface
{
    using Web;

    public interface INetBot
    {
        string Name { get; }
        string Color { get; }

        IEnumerable<BotletMove> GetMoves(MoveRequest request);
    }
}
