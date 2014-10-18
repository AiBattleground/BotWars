using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBots.WebServer.Repository
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        T Get(int id);
        T Add(T newItem);
        void Update(T updatedItem);
        void Delete(T itemToDelete);
    }
}
