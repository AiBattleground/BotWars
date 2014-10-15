using Newtonsoft.Json;
using GrahamBot;
using System.Web.Mvc;
using NetBots.Core;
using NetBots.Bot.Interface;

namespace NetBots.Bot.Host.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Index(string data)
        {
            var moveRequest = (JsonConvert.DeserializeObject<MoveRequest>(data));
            var moves = new Ai().GetMoves(moveRequest);
            return Json(moves);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
