using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBots.WebServer.Data.File
{
    using Repository;
    using Web;

    public class GameStateFileDao : IRepository<GameState>
    {
        public List<GameState> GetAll()
        {
            throw new NotImplementedException();
        }

        public GameState Get(int id)
        {
            throw new NotImplementedException();
        }

        public GameState Add(GameState newItem)
        {
            throw new NotImplementedException();
        }

        public void Update(GameState updatedItem)
        {
            throw new NotImplementedException();
        }

        public void Delete(GameState itemToDelete)
        {
            throw new NotImplementedException();
        }
    }
}
