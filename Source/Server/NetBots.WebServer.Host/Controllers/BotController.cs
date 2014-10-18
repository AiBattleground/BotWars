using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetBots.WebServer.Host.Controllers
{
    public class BotController : Controller
    {
        // GET: Bot
        public ActionResult Index()
        {
            return View();
        }
    }
}