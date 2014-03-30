using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using GrahamBot;

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
