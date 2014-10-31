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
            string defaultOwnerId = null;
            var defaultOwner = context.Users.FirstOrDefault();
            if (defaultOwner != null)
            {
                defaultOwnerId = defaultOwner.Id;
            }

            var divideByZero = UpsertPlayerBot(new PlayerBot()
            {
                Id = -1,
                Name = "DivideByZer0",
                OwnerId = defaultOwnerId,
                URL = "http://dividebyzer0.com",
            }, context);

            var grahamBot = UpsertPlayerBot(new PlayerBot()
            {
                Id = -1,
                Name = "GrahamBot",
                OwnerId = defaultOwnerId,
                URL = "http://graham.technology/bot",
            }, context);


            if (!context.GameSummaries.Any())
            {
                var match = new GameSummary()
                {
                    Player1 = grahamBot,
                    Player2 = divideByZero,
                    TournamentGame = false,
                    Winner = divideByZero
                };
                context.GameSummaries.Add(match);
            }

            context.SaveChanges();
            base.Seed(context);
        }

        private PlayerBot UpsertPlayerBot(PlayerBot bot, ApplicationDbContext context)
        {
            if (context.PlayerBots.Any(pb => pb.URL == bot.URL))
            {
                PlayerBot existingBot = context.PlayerBots.First(pb => pb.URL == bot.URL);

                context.Entry(existingBot).State = EntityState.Modified;

                existingBot.Name = bot.Name;
                existingBot.OwnerId = bot.OwnerId;
                return existingBot;
            }
            else
            {
                context.PlayerBots.Add(bot);
            }
            return bot;
        }
    }
}
