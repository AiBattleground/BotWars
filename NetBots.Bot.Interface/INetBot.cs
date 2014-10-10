using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBots.Bot.Interface
{
    public interface INetBot
    {
        string Name { get; }

        BotletMoveCollection GetMoves(GameState gameState);
    }
}
