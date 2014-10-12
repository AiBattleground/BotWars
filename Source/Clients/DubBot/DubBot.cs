
using NetBots.Bot.Interface;

namespace DubBot
{
    public class DubBot : INetBot
    {
        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public IBotLetMoveCollection GetMoves(GameState gameState)
        {
            throw new System.NotImplementedException();
        }
    }
}
