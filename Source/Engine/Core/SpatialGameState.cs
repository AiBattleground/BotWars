using System.Linq;
using System.Collections.Generic;

namespace NetBots.Core
{
    using Web;

    public class SpatialGameState : GameState
    {
        public SpatialGameState()
        {

        }

        public SpatialGameState(GameState gameState){
            Rows = gameState.Rows;
            Cols = gameState.Cols;
            P1 = gameState.P1;
            P2 = gameState.P2;
            Grid = gameState.Grid;
            MaxTurns = gameState.MaxTurns;
            TurnsElapsed = gameState.TurnsElapsed;
        }

        public Space GetSpace(int position){
            return new Space(position/Cols, position%Cols);
        }

        public int GetGridPosition(Space space){
            return space.y*Cols+space.x;
        }

        public bool IsInBounds(Space space)
        {
            bool xWithinBounds = space.x >= 0 && space.x < Cols;
            bool yWithinBounds = space.y >= 0 && space.y < Rows;
            return xWithinBounds && yWithinBounds;
        }

        public bool IsInBounds(int position)
        {
            return position >=0 && position<Rows*Cols;
        }

        public IEnumerable<Space> AdjacentSpaces(Space space)
        {
            List<Space> unBoundedAdjacentSpaces = new List<Space>()
			{ 
				new Space() { x = space.x + 1, y = space.y },
				new Space() { x = space.x, y = space.y - 1 },
				new Space() { x = space.x, y = space.y + 1 },
				new Space() { x = space.x - 1, y = space.y }
			};
            return unBoundedAdjacentSpaces.Where(s => IsInBounds(s));
        }

        public IEnumerable<int> AdjacentPositions(int position)
        {
            List<int> unBoundedAdjacentPositions = new List<int>()
            {
                position+1,
                position-1,
                position - Cols,
                position + Cols
            };
            return unBoundedAdjacentPositions.Where(p => IsInBounds(p));
        }
    }
}
