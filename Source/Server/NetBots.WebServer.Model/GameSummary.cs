using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBots.WebServer.Model
{
    public class GameSummary
    {
        public int Id { get; set; }
        public virtual PlayerBot Player1 { get; set; }
        public virtual PlayerBot Player2 { get; set; }
        public virtual PlayerBot Winner { get; set; }
        public bool TournamentGame { get; set; }

        public IEnumerable<PlayerBot> GetPlayers()
        {
            return new[] {Player1, Player2};
        } 
    }
}
