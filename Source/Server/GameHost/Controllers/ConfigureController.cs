using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameHost.Controllers
{
    public class ConfigureController : Controller
    {
        //
        // GET: /Configure/

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
