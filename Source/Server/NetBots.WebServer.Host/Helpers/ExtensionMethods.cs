using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Model;

namespace NetBotsHostProject.Helpers
{
    public static class ExtensionMethods
    {
        private static readonly Random MyRandom = new Random();

        public static IList<PlayerBot> GetVisibleBots(this ApplicationDbContext db, string userId)
        {
            var visibleBots = db.PlayerBots.Where(x => !x.Private || x.OwnerId == userId).ToList();
            return visibleBots;
        }

        public static IList<PlayerBot> GetVisibleBots(this ApplicationDbContext db)
        {
            var visibleBots = db.PlayerBots.Where(x => !x.Private).ToList();
            return visibleBots;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = MyRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}