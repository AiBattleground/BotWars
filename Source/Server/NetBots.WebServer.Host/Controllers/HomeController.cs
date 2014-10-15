using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using NetBots.WebServer.Host.Models;
using NetBots.Bot.Interface;
using NetBots.Core;
using NetBots.GameEngine;

namespace NetBots.WebServer.Host.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult NewGame()
        {
            string bot1Url = "http://localhost:1337/";
            string bot2Url = "http://localhost:1337/";

            GameState startingState = _GetNewGameState();
            Game game = new Game(startingState, _GetPlayers(20));

            for (int i = 0; i < 200; i++)
            {
                string p1Response = _GetBotletMoves("p1", bot1Url, game.GameState);
                string p2Response = _GetBotletMoves("p2", bot2Url, game.GameState);
                List<BotletMove> p1MoveList = JsonConvert.DeserializeObject<List<BotletMove>>
                    (p1Response);
                List<BotletMove> p2MoveList = JsonConvert.DeserializeObject<List<BotletMove>>
                    (p2Response);
                PlayerMoves p1Moves = new PlayerMoves() { Moves = p1MoveList, PlayerName = "p1" };
                PlayerMoves p2Moves = new PlayerMoves() { Moves = p2MoveList, PlayerName = "p2" };
                List<PlayerMoves> playersMoves = new List<PlayerMoves>(){ p1Moves, p2Moves };
                game.UpdateGameState(playersMoves);
                var hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WarViewHub>();
                hub.Clients.All.sendLatestMove(JsonConvert.SerializeObject(game.GameState));
                Thread.Sleep(100);
            }
            return Json("gameRunning");
        }
        
        private string _GetBotletMoves(string player, string botUrl, GameState state)
        {
            WebClient bot = new WebClient(); //should maybe save so not re-creating every move.
            MoveRequest moveRequest = new MoveRequest() { State = state, Player = player };
            string jsonMoveRequest = JsonConvert.SerializeObject(moveRequest);
            bot.QueryString = new NameValueCollection() { { "data", jsonMoveRequest } };
            return bot.UploadString(botUrl, "");
        }

        private GameSettings _GetGameSettings()
        {
            string[] userData;
            string dataFile = Server.MapPath("~/App_Data/GameSettings.json");
            if (System.IO.File.Exists(dataFile))
            {
                userData = System.IO.File.ReadAllLines(dataFile);
                if (userData == null)
                {
                    throw new Exception("The file is empty.");
                }
            }
            else
            {
                throw new Exception("The file does not exist.");
            }
            
            return JsonConvert.DeserializeObject<GameSettings>(string.Join("\n", userData));
        }

        private GameState _GetNewGameState()
        {
            GameSettings settings = _GetGameSettings();

            return new GameState()
            {
                Rows = settings.boardSize,
                Cols = settings.boardSize,
                P1 = new Player() { Energy = 1, Spawn = settings.boardSize + 1 },
                P2 = new Player() { Energy = 1, Spawn = settings.boardSize * (settings.boardSize - 1) - 2 },
                Grid = new string('.', settings.boardSize * settings.boardSize),
                MaxTurns = 200,
                TurnsElapsed = 0
            };
        }

        private IEnumerable<BotPlayer> _GetPlayers(int boardWidth){
            BotPlayer red = new BotPlayer()
			{
				PlayerName = "p1",
				BotletId = '1',
				Energy = 1,
				Spawn = boardWidth + 1,
				Resource = Resource.P1Botlet,
                deadBotletId = 'x'
			};
			BotPlayer blue = new BotPlayer(){
				PlayerName = "p2",
				BotletId = '2',
				Energy = 1,
				Spawn = boardWidth * (boardWidth - 1) - 2,
				Resource = Resource.P2Botlet,
                deadBotletId = 'X'
			};
			return new List<BotPlayer>(){red, blue};
        }
    }
}
