using Newtonsoft.Json;
using GrahamBot;
using System.Web.Mvc;
using NetBots.Core;
using NetBots.Bot.Interface;

namespace BotHost.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        
        public ActionResult Index(string data)
        {
            if (Request.HttpMethod == "POST")
                return Json(Ai.GetResponse(JsonConvert.DeserializeObject<MoveRequest>(data)));
            return View();
        }

    }
}
