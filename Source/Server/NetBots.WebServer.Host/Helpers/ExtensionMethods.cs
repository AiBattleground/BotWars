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
    }
}