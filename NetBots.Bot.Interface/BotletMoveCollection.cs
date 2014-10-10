using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBots.Bot.Interface
{
    public class BotletMoveCollection : IBotLetMoveCollection
    {
        private readonly List<BotletMove> _moves = new List<BotletMove>();

        public void Add(BotletMove move)
        {
            _moves.Add(move);
        }

        public void Remove(BotletMove move)
        {
            _moves.Remove(move);
        }

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
