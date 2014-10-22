using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetBots.WebServer.Host.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        // GET: History
        public ActionResult Index()
        {
            return View();
        }
    }
}