using Newtonsoft.Json;
using GrahamBot;
using System.Web.Mvc;
using NetBots.Core;
using NetBots.Bot.Interface;

namespace NetBots.Bot.Host.Controllers
{
    using Web;

    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Index(MoveRequest moveRequest)
        {
            var moves = new GrahamAi().GetMoves(moveRequest);

            return Json(moves);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
