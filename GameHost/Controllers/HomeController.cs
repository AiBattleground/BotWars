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
using GameHost.Models;
using NetBots.Bot.Interface;

namespace GameHost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult NewGame()
        {
            string bot1Url = "http://localhost:50046/";
            string bot2Url = "http://localhost:1337/";

            GameState startingState = _GetNewGameState();
            Game game = new Game(startingState, _GetPlayers(20));

            for (int i = 0; i < 200; i++)
            {
                string redResponse = _GetBotletMoves("r", bot1Url, game.GameState);
                string blueResponse = _GetBotletMoves("b", bot2Url, game.GameState);
                List<BotletMove> redMoves = JsonConvert.DeserializeObject<List<BotletMove>>
                    (redResponse);
                List<BotletMove> blueMoves = JsonConvert.DeserializeObject<List<BotletMove>>
                    (blueResponse);
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
                rows = settings.boardSize,
                cols = settings.boardSize,
                p1 = new Player() { energy = 1, spawn = settings.boardSize + 1 },
                p2 = new Player() { energy = 1, spawn = settings.boardSize * (settings.boardSize - 1) - 2 },
                grid = new string('.', settings.boardSize * settings.boardSize),
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
