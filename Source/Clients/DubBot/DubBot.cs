
using System.Collections;
using System.Collections.Generic;
using NetBots.Bot.Interface;
using NetBots.Web;

namespace DubBot
{
    public class DubBot : INetBot
    {
        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Color
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<BotletMove> GetMoves(MoveRequest moveRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
