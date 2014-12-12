﻿using System.Data.Entity;
﻿using System.Linq;
﻿using System.Net.Mail;
﻿using System.Web.Routing;
﻿using Microsoft.Ajax.Utilities;
﻿using Microsoft.AspNet.Identity;
﻿using NetBots.GameEngine;
﻿using NetBots.WebServer.Data.MsSql;
﻿using NetBots.WebServer.Host.Models;
﻿using NetBots.WebServer.Model;
﻿using NetBotsHostProject.Helpers;
﻿using System;
﻿using System.Threading.Tasks;
﻿using System.Web.Mvc;
﻿using NetBotsHostProject.Models;
﻿using SendGrid;

namespace NetBots.WebServer.Host.Controllers
{
    [Authorize]
    public class BattleController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return RedirectToAction("Skirmish");
        }

        public ActionResult Watch(string matchId)
        {
            return View();
        }

        public ActionResult Skirmish()
        {
            return View(_db.GetVisibleBots(User.Identity.GetUserId()));
        }

        public ActionResult Ladder()
        {
            var bots = _db.GetVisibleBots(User.Identity.GetUserId());
            var rankedBots = bots.Where(x => x.LadderRank != 0).OrderBy(x => x.LadderRank).ToList();
            var unrankedBots = bots.Where(x => x.LadderRank == 0);
            rankedBots.AddRange(unrankedBots); //Ensure unranked bots are listed last.
            return View(rankedBots);
        }

        public async Task<ActionResult> LadderMatch(int botId)
        {
            var myBot =
                await _db.PlayerBots.FirstOrDefaultAsync(
                    x => x.Id == botId && x.Owner.UserName == HttpContext.User.Identity.Name);
            if (myBot != null)
            {
                var latestLadderMatch =
                    _db.GameSummaries.Where(x => x.GameType == GameType.Ladder && 
                        x.Initiater.UserName == User.Identity.Name && 
                        (x.Player1.Id == botId || x.Player2.Id == botId))
                            .OrderByDescending(x => x.DateTime)
                            .FirstOrDefault();
                if (latestLadderMatch != null && DateTime.Now - latestLadderMatch.DateTime < TimeSpan.FromHours(1))
                {
                    var whenCanPlayAgain = (latestLadderMatch.DateTime + TimeSpan.FromHours(1)).ToShortTimeString();
                    ViewBag.ErrorText = "It has been less than an hour since your last ladder match.\r\n" +
                                        "You can play on the ladder again at " + whenCanPlayAgain;
                    return View("LadderMatch");
                }
                var enemyBot = await GetEnemyBot(myBot.LadderRank);
                if (enemyBot != null)
                {
                    var model = GetLadderMatchModel(myBot.Id, enemyBot.Id);
                    return View("LadderMatch", model);
                }
            }
            ViewBag.ErrorText = "Something went wrong trying to start the ladder match!";
            return View("LadderMatch");
        }

        private static LadderMatchViewModel GetLadderMatchModel(int playerId, int enemyId)
        {
            var p1 = new Random().Next(0, 2) == 1;
            if (p1)
            {
                return new LadderMatchViewModel()
                {
                    ChallengerId = playerId,
                    DefenderId = enemyId
                };
            }
            else
            {
                return new LadderMatchViewModel()
                {
                    ChallengerId = enemyId,
                    DefenderId = playerId
                };
            }
        }

        private async Task<PlayerBot> GetEnemyBot(int myRank)
        {
            if (myRank == 0)
            {
                return await _db.PlayerBots.OrderByDescending(x => x.LadderRank).FirstAsync();
            }
            else
            {
                return await _db.PlayerBots.FirstOrDefaultAsync(x => x.LadderRank == myRank - 1);
            }
        }


        
    }
}