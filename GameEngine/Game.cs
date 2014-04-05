using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotWars.Core;

namespace BotWars.GameEngine
{
    public class Game
    {
        public static GameState GetNextGameState
            (GameState previous, IEnumerable<BotletMove> redMoves, IEnumerable<BotletMove> blueMoves)
        {
            //check each players response is well formed
            //limit each players moves to the valid ones
            //assign bots to their new positions
            //raze bases
            //collect energies
            //fight battles
            //spawn bots
            //place energies
            return previous;
        }

    }
}
