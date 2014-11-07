﻿using System.Data.Entity;
using System.Web.Caching;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using NetBots.Core;
using NetBots.GameEngine;
using NetBots.Web;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Data.MsSql.Migrations;
using NetBots.WebServer.Host.Models;
using NetBots.WebServer.Model;
using NetBotsHostProject.Helpers;
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

namespace NetBots.WebServer.Host.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BattleController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        private const int TurnLimit = 200;

        public ActionResult Index()
        {
            return RedirectToAction("Skirmish");
        }

        [Route("battle/{bot1Name}/{bot2Name}")]
        public async Task<ActionResult> Index(string bot1Name, string bot2Name)
        {
            var bot1 = await _db.PlayerBots.FirstOrDefaultAsync(x => x.Name == bot1Name);
            var bot2 = await _db.PlayerBots.FirstOrDefaultAsync(x => x.Name == bot2Name);
            if (bot1 != null && bot2 != null)
            {
                var model = new BattleViewModel() { Bot1 = bot1, Bot2 = bot2 };
                return View(model);
            }
            throw new ArgumentException("Couldn't find one of the bots");
        }

        public ActionResult PartialOnly()
        {
            return View();
        }

        public ActionResult Skirmish()
        {
            return View(_db.GetVisibleBots(User.Identity.GetUserId()));
        }


        public async Task<ActionResult> NewGame(string bot1Url, string bot2Url)
        {

            GameState startingState = GetNewGameState();
            Game game = new Game(startingState, bot1Url, bot2Url);
            game.UpdateGameState(new List<PlayerMoves>()); //Do this get the starting bots to spawn.

            int currentTurn = 0;
            while (game.GameState.Winner == null && currentTurn < TurnLimit)
            {
                var myTasks = game.Players.Select(p => GetPlayerMovesAsync(p, game.GameState));
                var playersMoves = await Task.WhenAll(myTasks);
                game.UpdateGameState(playersMoves);
                var hub = GlobalHost.ConnectionManager.GetHubContext<WarViewHub>();
                hub.Clients.All.sendLatestMove(JsonConvert.SerializeObject(game.GameState));
                await Task.Delay(100);
                currentTurn++;
            }
            SaveGameResult(bot1Url, bot2Url, game);
            return new EmptyResult();
        }

        private void SaveGameResult(string bot1Url, string bot2Url, Game game)
        {
  
            var p1 = _db.PlayerBots.First(x => x.URL == bot1Url);
            var p2 = _db.PlayerBots.First(x => x.URL == bot2Url);
            if (String.IsNullOrWhiteSpace(game.GameState.Winner))
            {
                //If the game progressed to the turn limit, the winner won't be set yet, so we do it here.
                SetWinnerByBotCount(game); 
            }
            var gameResult = new GameSummary()
            {
                Player1 = p1,
                Player2 = p2,
                TournamentGame = false,
                Winner = game.GameState.Winner == "p1" ? p1 : game.GameState.Winner == "p2" ? p2 : null
            };
            _db.GameSummaries.Add(gameResult);
            _db.SaveChanges();
        }

        private static void SetWinnerByBotCount(Game game)
        {
            var p1Count = game.GameState.Grid.Count(x => x == '1');
            var p2Count = game.GameState.Grid.Count(x => x == '2');
            if (p1Count > p2Count)
                game.GameState.Winner = "p1";
            else if (p2Count > p1Count)
                game.GameState.Winner = "p2";
        }

        public static async Task<PlayerMoves> GetPlayerMovesAsync(BotPlayer player, GameState gameState)
        {
            MoveRequest moveRequest = new MoveRequest() { State = gameState, Player = player.PlayerName };
            string jsonMoveRequest = JsonConvert.SerializeObject(moveRequest);
            HttpClient client = GetClient(player.Uri);
            var content = new StringContent(jsonMoveRequest, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(player.Uri, content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var moves = JsonConvert.DeserializeObject<List<BotletMove>>(responseJson);
            var playerMove = new PlayerMoves() { Moves = moves, PlayerName = player.PlayerName };
            return playerMove;
        }

        private static HttpClient GetClient(string botUrl)
        {
            var cache = System.Web.HttpContext.Current.Cache;
            var client = cache[botUrl] as HttpClient;
            if (client == null)
            {
                client = new HttpClient();
                var oneMinute = new TimeSpan(0, 0, 1, 0);
                cache.Add(botUrl, client, null, Cache.NoAbsoluteExpiration, oneMinute, CacheItemPriority.High, null);
            }
            return client;
        }

        private GameSettings _GetGameSettings()
        {
            string[] userData;
            string dataFile = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/GameSettings.json");
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

        public GameState GetNewGameState()
        {
            GameSettings settings = _GetGameSettings();

            var startingGame = new GameState()
            {
                Rows = settings.boardSize,
                Cols = settings.boardSize,
                P1 = new Player() { Energy = 1, Spawn = settings.boardSize + 1 },
                P2 = new Player() { Energy = 1, Spawn = settings.boardSize * (settings.boardSize - 1) - 2 },
                Grid = new string('.', settings.boardSize * settings.boardSize),
                MaxTurns = 200,
                TurnsElapsed = 0,
                GameId = GameHelper.GenerateRandomGameId()
            };
            return startingGame;
        }

        

        
    }
}