using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NetBots.WebServer.Model;
using NetBots.WebServer.Host.Models;
using NetBots.WebServer.Data.MsSql;
using Microsoft.AspNet.Identity;
using NetBotsHostProject.Helpers;

namespace NetBotsHostProject.Controllers
{
    [Authorize]
    public class PlayerBotsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PlayerBots
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();
            var bots = db.GetVisibleBots(User.Identity.GetUserId());
            var models = bots.Select(x => GetPlayerBotViewModel(x, userName)).ToList();
            return View(models);
        }

        public ActionResult GameHistory(int botId)
        {
            var userName = User.Identity.GetUserName();
            var bot = db.PlayerBots.FirstOrDefault(x => x.Id == botId);
            var model = GetPlayerBotViewModel(bot, userName);
            return View(model);
        }

        private PlayerBotViewModel GetPlayerBotViewModel(PlayerBot bot, string userName)
        {
            var matchHistory = db.GameSummaries.Where(x => x.Player1.Id == bot.Id || x.Player2.Id == bot.Id).ToList();
            var model = new PlayerBotViewModel()
            {
                Id = bot.Id,
                Image = bot.Image,
                Name = bot.Name,
                Owner = bot.Owner != null ? bot.Owner.UserName : "",
                Rank = 0,
                OwnedByUser = bot.Owner != null && bot.Owner.UserName == userName,
                MatchHistory = matchHistory
            };
            return model;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
