using Microsoft.AspNet.SignalR;
using NetBots.Core;
using NetBots.GameEngine;
using NetBots.Web;
using NetBots.WebServer.Host.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.Hubs;
using NetBots.WebServer.Host.Hubs;
using NetBots.WebServer.Model;
using NetBots.WebServer.Data.MsSql;

namespace NetBots.WebServer.Host.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BattleController : Controller
    {
        private const string defaultBotUrl = "http://localhost:1337/";

        private string Bot1Url = defaultBotUrl;
        private string Bot2Url = defaultBotUrl;

        private readonly Dictionary<string, HttpClient> _clients;
        private ApplicationDbContext db;

        public BattleController()
        {
            _clients = new Dictionary<string, HttpClient>();
            db = new ApplicationDbContext();
        }

        public ActionResult Index(int bot1Id = 0, int bot2Id = 0)
        {
            var model = new BattleIndexViewModel
                {
                    Bot1Id = bot1Id,
                    Bot2Id = bot2Id
                };

            return View(model);
        }

        public async Task<ActionResult> NewGame(int bot1Id, int bot2Id)
        {
            _SetBotUrls(bot1Id, bot2Id);

            GameState startingState = _GetNewGameState();
            Game game = new Game(startingState, _GetPlayers(20));

            for (int i = 0; i < 200; i++)
            {
                var myTasks = game.Players.Select(p => GetAllPlayerMovesAsync(p, game.GameState));
                var playersMoves = await Task.WhenAll(myTasks);
                game.UpdateGameState(playersMoves);
                var hub = GlobalHost.ConnectionManager.GetHubContext<WarViewHub>();
                hub.Clients.All.sendLatestMove(JsonConvert.SerializeObject(game.GameState));
                Thread.Sleep(100);
            }

            return new EmptyResult();
        }

        private async Task<PlayerMoves> GetAllPlayerMovesAsync(BotPlayer player, GameState gameState)
        {
            var moves = await GetBotletMovesAsync(player, gameState);
            var playerMove = new PlayerMoves() { Moves = moves, PlayerName = player.PlayerName };

            return playerMove;
        }

        private async Task<List<BotletMove>> GetBotletMovesAsync(BotPlayer player, GameState state)
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

        private void _SetBotUrls(int bot1id, int bot2id)
        {
            PlayerBot bot1 = db.PlayerBots.FirstOrDefault(pb => pb.Id == bot1id);

            if (bot1 == null || string.IsNullOrWhiteSpace(bot1.URL))
            {
                Bot1Url = defaultBotUrl;
            }
            else
            {
                Bot1Url = bot1.URL;
            }

            PlayerBot bot2 = db.PlayerBots.FirstOrDefault(pb => pb.Id == bot2id);
            
            if (bot2 == null || string.IsNullOrWhiteSpace(bot2.URL))
            {
                Bot2Url = defaultBotUrl;
            }
            else
            {
                Bot2Url = bot2.URL;
            }
        }

        private IEnumerable<BotPlayer> _GetPlayers(int boardWidth)
        {
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
            BotPlayer blue = new BotPlayer()
            {
                PlayerName = "p2",
                BotletId = '2',
                Energy = 1,
                Uri = Bot2Url,
                Spawn = boardWidth * (boardWidth - 1) - 2,
                Resource = Resource.P2Botlet,
                deadBotletId = 'X'
            };
            return new List<BotPlayer>() { red, blue };
        }
    }
}