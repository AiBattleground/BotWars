using System.Data.Entity.Core.Objects;
using Microsoft.AspNet.Identity.EntityFramework;
using NetBots.WebServer.Data.MsSql.Migrations;
using NetBots.WebServer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBots.WebServer.Data.MsSql
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PlayerBot> PlayerBots { get; set; }
        public DbSet<GameSummary> GameSummaries { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
