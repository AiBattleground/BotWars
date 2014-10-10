using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBots.Bot.Interface
{
    public class BotletMoveCollection : IEnumerable<BotletMove>
    {
        private List<BotletMove> _moves;

        IEnumerator<BotletMove> IEnumerable<BotletMove>.GetEnumerator()
        {
            return _moves.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _moves.GetEnumerator();
        }
    }
}
