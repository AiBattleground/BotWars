using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NetBots.GameEngine;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Model;
using NetBotsHostProject.Helpers;
using NetBotsHostProject.Hubs;
using SendGrid;

namespace NetBots.WebServer.Host.Hubs
{
    public class WarViewHub : Hub
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();

        public async Task<string> StartSkirmish(int bot1Id, int bot2Id)
        {
            var bot1 = _db.PlayerBots.FirstOrDefault(x => x.Id == bot1Id);
            var bot2 = _db.PlayerBots.FirstOrDefault(x => x.Id == bot2Id);
            var userId = Context.User.Identity.Name;
            var user = _db.Users.FirstOrDefault(x => x.UserName == userId);
            var gameManager = new WarGameManager();
            var gameId = GameHelper.GenerateRandomGameId();
            var winner = await gameManager.RunGame(gameId, bot1, bot2, x => Clients.Caller.sendLatestMove(x));
            
            SaveGameSummary(bot1, bot2, winner, GameType.Skirmish, user);
            return winner.Name;
        }

        public async Task<string> StartLadderMatch(int cahllengerId, int defenderId)
        {
            var challengerBot = _db.PlayerBots.FirstOrDefault(x => x.Id == cahllengerId);
            var defenderBot = _db.PlayerBots.FirstOrDefault(x => x.Id == defenderId);
            if (challengerBot == null || defenderBot == null)
            {
                throw new Exception("Could not find bots.");
            }
            if (Math.Abs(challengerBot.LadderRank - defenderBot.LadderRank) == 1 || challengerBot.LadderRank == 0 || defenderBot.LadderRank == 0)
            {
                var gameId = GameHelper.GenerateRandomGameId(); 
                List<PlayerBot> bothBots = new List<PlayerBot>() { challengerBot, defenderBot };
                bothBots.Shuffle();
                await Groups.Add(Context.ConnectionId, gameId);
                //SendNotificationEmail(defenderBot.URL, defenderBot.Name, gameState.GameId);
                await CountDown(3, x => Clients.Group(gameId).countDown(x));
                var gameManager = new WarGameManager();

                var winner = await gameManager.RunGame(gameId, bothBots[0], bothBots[1], x => Clients.Group(gameId).sendLatestMove(x));
                SaveGameSummary(bothBots[0], bothBots[1], winner, GameType.Ladder, challengerBot.Owner);
                UpdateLadderRankings(winner, (new[] {bothBots[0], bothBots[1]}).First(x => x.Id != winner.Id));
                return winner.Name;
            }
            return null;
        }

        public async Task CountDown(int seconds, Action<int> updateAction)
        {
            while (seconds > 0)
            {
                seconds--;
                await Task.Delay(TimeSpan.FromSeconds(1));
                updateAction(seconds);
            }
        }

        private void UpdateLadderRankings(PlayerBot winner, PlayerBot loser)
        {
            var bothBots = new[] {winner, loser};
            var highRank = bothBots.Where(x => x.LadderRank != 0).Min(x => x.LadderRank);
            int lowRank;
            if (bothBots.Any(x => x.LadderRank == 0))
            {
                lowRank = _db.PlayerBots.Count(x => x.LadderRank != 0) + 1;
            }
            else
            {
                lowRank = bothBots.Max(x => x.LadderRank);
            }
            winner.LadderRank = highRank;
            loser.LadderRank = lowRank;
            _db.SaveChanges();
        }

        private void SaveGameSummary(PlayerBot bot1, PlayerBot bot2, PlayerBot winner, GameType gameType, ApplicationUser initiator)
        {
            var match = new GameSummary()
            {
                DateTime = DateTime.Now,
                Player1 = bot1,
                Player2 = bot2,
                Winner = winner,
                GameType = gameType,
                Initiater = initiator
            };
            _db.GameSummaries.Add(match);
        }

        private void SendNotificationEmail(string address, string botName, string matchId)
        {
            var message = new SendGridMessage();
            message.From = new MailAddress("no-reply@aibattleground.com");
            message.AddTo(address);
            message.Subject = botName + " has is being challenged on the Ladder!";
            var url = "http://aibattleground.com/Battle/Watch/" + matchId;
            var linkText = String.Format("<a href=\"{0}\">Click here to watch</a>", url);
            message.Html = String.Format("{0} has been challenged to a ladder match. The game will begin in 30 seconds! {1}", botName, linkText);
        }
    }

    
   
}