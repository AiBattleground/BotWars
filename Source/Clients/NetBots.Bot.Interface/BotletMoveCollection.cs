using System.Collections.Generic;
using System.Linq;

namespace NetBots.Bot.Interface
{
    public class BotletMoveCollection : IBotLetMoveCollection
    {
        private List<BotletMove> _moves = new List<BotletMove>();

        public ICollection<BotletMove> Moves { get { return _moves; } set { _moves = value.ToList(); } }
        public string Color { get; set; }
    }
}
