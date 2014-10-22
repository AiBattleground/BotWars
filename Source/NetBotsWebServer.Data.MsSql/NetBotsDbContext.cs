using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace NetBots.WebServer.Data.MsSql
{
    using Model;

    public class NetBotsDbContext : DbContext
    {
        public DbSet<PlayerBot> PlayerBots { get; set; }
        public DbSet<BotRecord> BotRecords { get; set; }
    }
}
