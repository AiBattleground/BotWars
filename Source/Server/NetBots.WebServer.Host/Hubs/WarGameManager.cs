using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using NetBots.EngineModels;
using NetBots.GameEngine;
using NetBots.WebModels;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Host.Hubs;
using NetBots.WebServer.Host.Models;
using NetBots.WebServer.Model;
using NetBotsHostProject.Models;
using Newtonsoft.Json;

namespace NetBotsHostProject.Hubs
{
    public class WarGameManager
    {
        private const int TurnLimit = 200;

        public async Task<PlayerBot> RunGame(string gameId, PlayerBot bot1, PlayerBot bot2, Action<WarViewModel> updateClient)
        {
            Game game = null;
            try
            {
                if (bot1 != null && bot2 != null)
                {
                    var gameState = GetNewGameState(gameId);
                    game = new Game(gameState, bot1.URL, bot2.URL);

                    int currentTurn = 0;
                    while (game.GameState.Winner == null && currentTurn < TurnLimit)
                    {
                        var delay = Task.Delay(250);
                        var myTasks = game.Players.Select(p => GetPlayerMovesAsync(p, game.GameState));
                        var httpMoves = await Task.WhenAll(myTasks);
                        game.UpdateGameState(httpMoves.Select(x => x.PlayerMoves));
                        var model = GetWarViewModel(game, bot1.Name, bot2.Name);
                        if (!ValidateMoves(httpMoves, game))
                        {
                            model.Alert = GetAlert(httpMoves);
                        }
                        updateClient(model);
                        await delay;
                        currentTurn++;
                    }
                    var winnerBot = GetWinningBot(game, bot1, bot2);
                    var finalModel = GetWarViewModel(game, bot1.Name, bot2.Name);
                    updateClient(finalModel);
                    await Task.WhenAll(game.Players.Select(p => GetPlayerMovesAsync(p, game.GameState)));
                    return winnerBot;
                }
                return null;
            }
            catch (Exception ex)
            {
                if (game != null && game.GameState.Winner != null)
                {
                    var winner = GetWinningBot(game, bot1, bot2);
                    return winner;
                }
                var e2 = ex;
                return null;
            }
        }



        private string GetAlert(HttpMove[] httpMoves)
        {
            var alert = "";
            foreach (var httpMove in httpMoves.Where(x => x.HttpMoveResponse == HttpMoveResponse.Error))
            {
                alert += httpMove.PlayerMoves.PlayerName + " returned an error:\r\n";
                alert += httpMove.Exception.Message + "\r\n";
            }
            return alert;
        }

        private bool ValidateMoves(HttpMove[] httpMoves, Game game)
        {
            if (httpMoves.Any(x => x.HttpMoveResponse == HttpMoveResponse.Error))
            {
                //If both returned an error, don't set a winner. 
                if (httpMoves.Any(x => x.HttpMoveResponse != HttpMoveResponse.Error))
                {
                    var badOne = httpMoves.First(x => x.HttpMoveResponse == HttpMoveResponse.Error);
                    var winnerName = badOne.PlayerMoves.PlayerName.ToLower() == "p1" ? "p2" : "p1";
                    game.GameState.Winner = winnerName;
                }
                return false;
            }
            return true;
        }

        public static WarViewModel GetWarViewModel(Game game, string p1Name, string p2Name)
        {
            var model = new WarViewModel()
            {
                P1Name = p1Name,
                P2Name = p2Name,
                State = game.GameState
            };
            return model;
        }

        private static PlayerBot GetWinningBot(Game game, PlayerBot bot1, PlayerBot bot2)
        {
            if (String.IsNullOrWhiteSpace(game.GameState.Winner))
            {
                SetWinnerByBotCount(game);
            }
            if (game.GameState.Winner == "p1")
            {
                return bot1;
            }
            else if (game.GameState.Winner == "p2")
            {
                return bot2;
            }
            else
            {
                return null;
            }
        }

        //If the game progressed to the turn limit, we set the winner by number of bots
        private static void SetWinnerByBotCount(Game game)
        {
            var p1Count = game.GameState.Grid.Count(x => x == '1');
            var p2Count = game.GameState.Grid.Count(x => x == '2');
            if (p1Count > p2Count)
            {
                game.GameState.Winner = "p1";
            }
            else if (p2Count > p1Count)
            {
                game.GameState.Winner = "p2";
            }
        }

        public async Task<HttpMove> GetPlayerMovesAsync(BotPlayer player, GameState gameState)
        {
            try
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
                var httpMove = new HttpMove()
                {
                    HttpMoveResponse = HttpMoveResponse.OK,
                    PlayerMoves = playerMove
                };
                return httpMove;
            }
            catch (TaskCanceledException ex)
            {
                //This means the Http Client timed out. We return an empy move list instead.
                return GetEmptyHttpMove(player.PlayerName, HttpMoveResponse.Timeout);
            }
            catch (HttpRequestException ex)
            {
                //This means a 400 returned or something like that, which is a disqualification.
                return GetEmptyHttpMove(player.PlayerName, HttpMoveResponse.Error, ex);
            }
        }

        private static HttpMove GetEmptyHttpMove(string pName, HttpMoveResponse response, HttpRequestException ex = null)
        {
            var pMove = new PlayerMoves()
            {
                Moves = new BotletMove[0],
                PlayerName = pName
            };
            var httpMove = new HttpMove()
            {
                PlayerMoves = pMove,
                HttpMoveResponse = response,
                Exception = ex
            };
            return httpMove;
        }

        private static HttpClient GetClient(string botUrl)
        {
            var cache = HttpContext.Current.Cache;
            var client = cache[botUrl] as HttpClient;
            if (client == null)
            {
                client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3); //Give the bot time to "wake up" on first call
                cache.Add(botUrl, client, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5),
                    CacheItemPriority.High, null);
            }
            return client;
        }

        private static GameSettings GetGameSettings()
        {
            string[] userData;
            string dataFile = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/GameSettings.json");
            if (System.IO.File.Exists(dataFile))
            {
                userData = System.IO.File.ReadAllLines(dataFile);
                if (userData.Length == 0)
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

        public static GameState GetNewGameState(string gameId)
        {
            GameSettings settings = GetGameSettings();

            var startingGame = new GameState()
            {
                Rows = settings.boardSize,
                Cols = settings.boardSize,
                P1 = new Player() { Energy = 1, Spawn = settings.boardSize + 1 },
                P2 = new Player() { Energy = 1, Spawn = settings.boardSize * (settings.boardSize - 1) - 2 },
                Grid = new string('.', settings.boardSize * settings.boardSize),
                MaxTurns = 200,
                TurnsElapsed = 0,
                GameId = gameId
            };
            return startingGame;
        }

        public Game GetGame(GameState gamestate, string side1Url, string side2Url, int seed)
        {
            if (seed != 0)
            {
                return new Game(gamestate, side1Url, side2Url);
            }
            else
            {
                return new Game(gamestate, side1Url, side2Url, seed);
            }
        }
    }


}