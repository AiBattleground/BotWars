using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Host.Models;

namespace NetBots.WebServer.Host.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db;

        public HomeController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Skirmish()
        {
            var bots = db.PlayerBots.ToList();
            var model = new SkirmishViewModel(bots);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Skirmish(int SelectedBot1Id = 0, int SelectedBot2Id = 0)
        {
            return RedirectToAction("Index", "Battle", new
                {
                    bot1Id = SelectedBot1Id,
                    bot2Id = SelectedBot2Id
                });
        }
    }
}