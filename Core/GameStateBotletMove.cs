using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotWars.Core
{
    class GameStateBotletMove
    {
        public SpatialGameState State { get; set; }
        public BotletMove Move { get; set; }

        public GameStateBotletMove(GameState state, BotletMove move)
        {
            State = new SpatialGameState(state);
            Move = move;
        }

        public Space From()
        {
            return State.GetSpace(Move.from);
        }

        public Space To()
        {
            return State.GetSpace(Move.to);
        }

        public bool IsContiguous(){
            return State.AdjacentSpaces(To()).Contains(From());
        }

        public bool IsValid()
        {
            return State.IsInBounds(From()) && State.IsInBounds(To()) && IsContiguous();
        }
    }
}
