using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using NetBots.WebServer.Model;

namespace NetBots.WebServer.Data.MsSql.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "NetBots.WebServer.Host.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            UpsertPlayerBot(new PlayerBot
                {
                    Id = -1,
                    Name = "DivideByZer0",
                    Owner = "Dubman",
                    URL = "http://dividebyzer0.com",
                    Wins = 3,
                    Losses = 0,
                    Ties = 0
                }, context);

            UpsertPlayerBot(new PlayerBot
            {
                Id = -1,
                Name = "GrahamBot",
                Owner = "Pabreetzio",
                URL = "http://graham.technology/bot",
                Wins = 2,
                Losses = 1,
                Ties = 0
            }, context);

            base.Seed(context);
        }

        private void UpsertPlayerBot(PlayerBot bot, ApplicationDbContext context)
        {
            if (context.PlayerBots.Any(pb => pb.URL == bot.URL))
            {
                PlayerBot existingBot = context.PlayerBots.First(pb => pb.URL == bot.URL);

                context.Entry(existingBot).State = EntityState.Modified;

                existingBot.Name = bot.Name;
                existingBot.Owner = bot.Owner;
                existingBot.Wins = bot.Wins;
                existingBot.Losses = bot.Losses;
                existingBot.Ties = bot.Ties;
            }
            else
            {
                context.PlayerBots.Add(bot);
            }

            context.SaveChanges();
        }
    }
}
