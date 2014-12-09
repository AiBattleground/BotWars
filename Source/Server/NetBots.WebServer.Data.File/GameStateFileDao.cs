using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetBots.WebModels;
using NetBots.WebServer.Repository;

namespace NetBots.WebServer.Data.File
{

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
