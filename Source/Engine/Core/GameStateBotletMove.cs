using System.Linq;

namespace NetBots.Core
{
    using Web;

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
            return State.GetSpace(Move.From);
        }

        public Space To()
        {
            return State.GetSpace(Move.To);
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
