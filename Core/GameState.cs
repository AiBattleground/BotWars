using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotWars.Core
{
    public class GameState
    {
        public int rows { get; set; }
        public int cols { get; set; }
        public Player p1 { get; set; }
        public Player p2 { get; set; }
        public string grid { get; set; }
        public int maxTurns { get; set; }
        public int turnsElapsed { get; set; }
    }
}
