using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using BotWars.Core;
using BotWars.GameEngine;

namespace GameHost.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult NewGame()
        {
            string bot1Url = "http://graham.technology/Bot";
            string bot2Url = "http://localhost:1337/";
            GameState startingState = _GetNewGameState(12);
            _GetBotletMoves("p1", bot1Url, startingState);
            _GetBotletMoves("p2", bot2Url, startingState);
            return Json("gameRunning");
        }

        private string _GetBotletMoves(string player, string botUrl, GameState state)
        {
            WebClient bot = new WebClient(); //should maybe save so not re-creating every move.
            MoveRequest moveRequest = new MoveRequest() { state = state, player = player };
            string jsonMoveRequest = JsonConvert.SerializeObject(moveRequest);
            bot.QueryString = new NameValueCollection() { { "data", jsonMoveRequest } };
            return bot.UploadString(botUrl, "");
        }

        private GameState _GetNewGameState(int boardWidth)
        {
            return new GameState()
            {
                rows = boardWidth,
                cols = boardWidth,
                p1 = new Player() { energy = 1, spawn = boardWidth + 1 },
                p2 = new Player() { energy = 1, spawn = boardWidth * (boardWidth - 1) - 2 },
                grid = new string('.', boardWidth * boardWidth),
                maxTurns = 200,
                turnsElapsed = 0
            };
        }
    }
}
