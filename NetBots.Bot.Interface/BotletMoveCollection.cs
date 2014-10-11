using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace NetBots.Bot.Interface
{
    public class BotletMoveCollection : IBotLetMoveCollection
    {
        private readonly List<BotletMove> _moves = new List<BotletMove>(); 

        public IEnumerable<BotletMove> Moves { get { return _moves; } }
        public string Color { get; set; }
    }
}
