using System.Collections.ObjectModel;
using NetBots.Bot.Interface;
using NetBots.Core;

namespace GrahamBot
{
    using NetBots.Web;

    public class Grid : Collection<Collection<Resource>>
    {
        public Space RedSpawn { get; private set; }
        public Space BlueSpawn { get; private set; }
        public char[] AsCharacterArray { get; private set; }
        public char[] ToCharArray() { return AsCharacterArray; }
        public int rows { get; private set; }
        public int cols { get; private set; }

        public Grid(GameState gameState)
        {
            AsCharacterArray = gameState.Grid.ToCharArray();
            rows = gameState.Rows;
            cols = gameState.Cols;
            for (int y = 0; y < gameState.Cols; y++)
            {
                Collection<Resource> row = new Collection<Resource>();
                for (int x = 0; x < rows; x++)
                {
                    row.Add((Resource)gameState.Grid.ToCharArray()[y * cols + x]);
                }
                this.Add(row);
            }
            RedSpawn = GetSpace(gameState.P1.Spawn);
            BlueSpawn = GetSpace(gameState.P2.Spawn);
        }

        public Space GetSpace(int space){
            return new Space(space % cols, space/cols);
        }

        public Collection<Space> GetSpaces(Resource resource)
        {
            Collection<Space> resourceSpaces = new Collection<Space>();
            foreach (Collection<Resource> row in this)
            {
                foreach (Resource space in row)
                {
                    if (space == resource)
                        resourceSpaces.Add(new Space() { y = this.IndexOf(row), x = row.IndexOf(space) });
                }
            }
            return resourceSpaces;
        }

        public BotletMove GetMove(Space from, Space to)
        {
            return new BotletMove() {
                From = (from.y*this.Count + from.x),
                To = (to.y * this.Count + to.x),
            };
        }
    }
}