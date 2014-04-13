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
using Microsoft.AspNet.SignalR;

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
            GameState startingState = _GetNewGameState(20);
            Game game = new Game(startingState, _GetPlayers(20));
            for (int i = 0; i < 200; i++)
            {
                List<BotletMove> redMoves = JsonConvert.DeserializeObject<List<BotletMove>>
                    (_GetBotletMoves("r", bot1Url, game.GameState));
                List<BotletMove> blueMoves = JsonConvert.DeserializeObject<List<BotletMove>>
                    (_GetBotletMoves("b", bot2Url, game.GameState));
                PlayerMoves p1Moves = new PlayerMoves() { Moves = redMoves, PlayerName = "p1" };
                PlayerMoves p2Moves = new PlayerMoves() { Moves = blueMoves, PlayerName = "p2" };
                List<PlayerMoves> playersMoves = new List<PlayerMoves>(){ p1Moves, p2Moves };
                game.UpdateGameState(playersMoves);
                GlobalHost.ConnectionManager.GetHubContext<Hubs.WarViewHub>()
                    .Clients.All.sendLatestMove(JsonConvert.SerializeObject(game.GameState));
            }
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

        private IEnumerable<BotPlayer> _GetPlayers(int boardWidth){
            BotPlayer red = new BotPlayer()
			{
				color = "red",
				playerName = "p1",
				botletId = 'r',
				energy = 1,
				spawn = boardWidth + 1,
				resource = Resource.RedBotlet,
                deadBotletId = 'x'
			};
			BotPlayer blue = new BotPlayer(){
				color = "blue",
				playerName = "p2",
				botletId = 'b',
				energy = 1,
				spawn = boardWidth * (boardWidth - 1) - 2,
				resource = Resource.BlueBotlet,
                deadBotletId = 'X'
			};
			return new List<BotPlayer>(){red, blue};
        }
    }
}
