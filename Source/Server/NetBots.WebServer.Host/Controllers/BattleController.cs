﻿using System.Data.Entity;
﻿using System.Net;
﻿using System.Web.Caching;
﻿using System.Web.Http;
﻿using System.Web.Http.Results;
﻿using Microsoft.Ajax.Utilities;
﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using NetBots.Core;
using NetBots.GameEngine;
using NetBots.Web;
using NetBots.WebServer.Data.MsSql;
﻿using NetBots.WebServer.Host.Models;
using NetBots.WebServer.Model;
using NetBotsHostProject.Helpers;
﻿using NetBotsHostProject.Models;
﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
﻿using System.Threading.Tasks;
﻿using System.Web.Mvc;
﻿using NetBots.WebServer.Host.Hubs;

namespace NetBots.WebServer.Host.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BattleController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return RedirectToAction("Skirmish");
        }

        [System.Web.Mvc.Route("battle/{bot1Name}/{bot2Name}")]
        public async Task<ActionResult> Index(string bot1Name, string bot2Name)
        {
            var bot1 = await _db.PlayerBots.FirstOrDefaultAsync(x => x.Name == bot1Name);
            var bot2 = await _db.PlayerBots.FirstOrDefaultAsync(x => x.Name == bot2Name);
            if (bot1 != null && bot2 != null)
            {
                var model = new BattleViewModel() { Bot1 = bot1, Bot2 = bot2 };
                return View(model);
            }
            throw new ArgumentException("Couldn't find one of the bots");
        }

        public ActionResult PartialOnly()
        {
            return View();
        }

        public ActionResult Skirmish()
        {
            return View(_db.GetVisibleBots(User.Identity.GetUserId()));
        }
        

        
    }
}