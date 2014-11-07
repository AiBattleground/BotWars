using Microsoft.AspNet.Identity;
using NetBots.WebServer.Data.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetBotsHostProject.Helpers;

namespace NetBots.WebServer.Host.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }

        public ActionResult Instructions()
        {
            return View();
        }

        public ActionResult Schedule()
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
    }
}