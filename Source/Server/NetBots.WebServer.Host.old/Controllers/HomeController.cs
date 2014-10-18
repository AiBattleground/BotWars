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
    using NetBots.Web;

    public class HomeController : Controller
    {
        private const string Bot1Url = "http://localhost:1337/";
        private const string Bot2Url = "http://localhost:1337/";

        //Url for starter kit bot.
        //private const string Bot2Url = "http://localhost:59345/api/Bot";

        //private const string Bot1Url = "http://randombot.azurewebsites.net/api/Bot";
        //private const string Bot2Url = "http://randombot.azurewebsites.net/api/Bot";

        private readonly Dictionary<string, HttpClient> _clients;

        public HomeController()
        {
            _clients = new Dictionary<string, HttpClient>();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> NewGame()
        {
            GameState startingState = _GetNewGameState();
            Game game = new Game(startingState, _GetPlayers(20));

            for (int i = 0; i < 200; i++)
            {
                var myTasks = game.Players.Select(p => GetAllPlayerMovesAsync(p, game.GameState));
                var playersMoves = await Task.WhenAll(myTasks);
                game.UpdateGameState(playersMoves);
                var hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WarViewHub>();
                hub.Clients.All.sendLatestMove(JsonConvert.SerializeObject(game.GameState));
                Thread.Sleep(100);
            }

            return Json("Game Over!");
        }

        private async Task<PlayerMoves> GetAllPlayerMovesAsync(BotPlayer player, GameState gameState)
        {
            var moves = await GetBotletMovesAsync(player, gameState);
            var playerMove = new PlayerMoves() { Moves = moves, PlayerName = player.PlayerName };

            return playerMove;
        }
        
        private async Task<List<BotletMove>>  GetBotletMovesAsync(BotPlayer player, GameState state)
        {
            MoveRequest moveRequest = new MoveRequest() { State = state, Player = player.PlayerName };
            string jsonMoveRequest = JsonConvert.SerializeObject(moveRequest);
            HttpClient client = GetClient(player.Uri);
            var content = new StringContent(jsonMoveRequest, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(player.Uri, content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var move = JsonConvert.DeserializeObject<List<BotletMove>>(responseJson);
            return move;
        }

        private HttpClient GetClient(string botUrl)
        {
            if (!_clients.ContainsKey(botUrl))
            {
                var client = new HttpClient();
                _clients.Add(botUrl, client);
            }

            return _clients[botUrl];
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
                Uri = Bot1Url,
				Spawn = boardWidth + 1,
				Resource = Resource.P1Botlet,
                deadBotletId = 'x'
			};
			BotPlayer blue = new BotPlayer(){
				PlayerName = "p2",
				BotletId = '2',
				Energy = 1,
                Uri = Bot2Url,
				Spawn = boardWidth * (boardWidth - 1) - 2,
				Resource = Resource.P2Botlet,
                deadBotletId = 'X'
			};
			return new List<BotPlayer>(){red, blue};
        }
    }
}
