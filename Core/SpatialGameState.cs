using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotWars.Core
{
    public class SpatialGameState : GameState
    {
        public SpatialGameState()
        {

        }
        public SpatialGameState(GameState gameState){
            rows = gameState.rows;
            cols = gameState.cols;
            p1 = gameState.p1;
            p2 = gameState.p2;
            grid = gameState.grid;
            maxTurns = gameState.maxTurns;
            turnsElapsed = gameState.turnsElapsed;
        }
        public Space GetSpace(int position){
            return new Space(position/cols, position%cols);
        }
        public int GetGridPosition(Space space){
            return space.y*cols+space.x;
        }

        public bool IsInBounds(Space space)
        {
            bool xWithinBounds = space.x >= 0 && space.x < cols;
            bool yWithinBounds = space.y >= 0 && space.y < rows;
            return xWithinBounds && yWithinBounds;
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
    }
}
