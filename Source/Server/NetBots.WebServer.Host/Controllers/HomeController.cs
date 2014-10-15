using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<ActionResult> NewGame()
        {
            string bot1Url = "http://localhost:1337/";
            string bot2Url = "http://localhost:1337/";
            //Url for Berserker sample bot.
            //string bot2Url = "http://localhost:59345/api/Bot";

            GameState startingState = _GetNewGameState();
            Game game = new Game(startingState, _GetPlayers(20));

            for (int i = 0; i < 200; i++)
            {
                var p1Response = GetBotletMovesAsync("p1", bot1Url, game.GameState);
                var p2Response = GetBotletMovesAsync("p2", bot2Url, game.GameState);
                var p1ResponseMoves = await p1Response;
                var p2ResponseMoves = await p2Response;
                PlayerMoves p1Moves = new PlayerMoves() { Moves = p1ResponseMoves, PlayerName = "p1" };
                PlayerMoves p2Moves = new PlayerMoves() { Moves = p2ResponseMoves, PlayerName = "p2" };
                List<PlayerMoves> playersMoves = new List<PlayerMoves>(){ p1Moves, p2Moves };
                game.UpdateGameState(playersMoves);
                var hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WarViewHub>();
                hub.Clients.All.sendLatestMove(JsonConvert.SerializeObject(game.GameState));
                Thread.Sleep(100);
            }
            return Json("Game Over!");
        }

        //private static List<BotletMove> _GetBotletMoves(string player, string botUrl, GameState state)
        //{
        //    MoveRequest moveRequest = new MoveRequest() { State = state, Player = player };
        //    string jsonMoveRequest = JsonConvert.SerializeObject(moveRequest);
        //    var client = new HttpClient();
        //    var response = client.PostAsync(botUrl, new StringContent(jsonMoveRequest, Encoding.UTF8, "application/json")).Result;
        //    response.EnsureSuccessStatusCode();
        //    var responseJson = response.Content.ReadAsStringAsync().Result;
        //    var move = JsonConvert.DeserializeObject<List<BotletMove>>(responseJson);
        //    return move;
        //}
        
        private static async Task<List<BotletMove>>  GetBotletMovesAsync(string player, string botUrl, GameState state)
        {
            MoveRequest moveRequest = new MoveRequest() { State = state, Player = player };
            string jsonMoveRequest = JsonConvert.SerializeObject(moveRequest);
            var client = new HttpClient();
            var response = await client.PostAsync(botUrl, new StringContent(jsonMoveRequest, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var move = JsonConvert.DeserializeObject<List<BotletMove>>(responseJson);
            return move;
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
