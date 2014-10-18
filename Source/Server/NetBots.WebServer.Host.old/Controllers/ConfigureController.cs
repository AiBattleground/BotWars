using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetBots.WebServer.Host.Controllers
{
    public class ConfigureController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateGameSettings(string gameSettings)
        {
            var dataFile = Server.MapPath("~/App_Data/GameSettings.json");
            System.IO.File.WriteAllText(dataFile, gameSettings);

            return Json("Information saved.");
        }
    }
}
