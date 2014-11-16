using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Ajax.Utilities;
using NetBots.Core;
using NetBots.GameEngine;
using NetBots.Web;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Host.Controllers;
using NetBots.WebServer.Host.Models;
using NetBots.WebServer.Model;
using NetBotsHostProject.Helpers;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace NetBotsHostProject.Controllers
{
    [EnableCors("*", "*", "*")]
    public class NetBotApiController : ApiController
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();

        [Route("api/botlist")]
        public IHttpActionResult GetBotList()
        {
            var visibleBots = _db.GetVisibleBots();
            var returnObjects = visibleBots.Select(x => new BotIdApiModel() { Name = x.Name, Id = x.Id });
            return Ok(returnObjects);
        }

        [Route("api/startgame")]
        [HttpPost]
        public async Task<IHttpActionResult> StartGame(StartGameApiModel apiModel)
        {
            var battleController = new BattleController();
            var gamestate = battleController.GetNewGameState();
            var p1Url = await GetPlayerUrl(apiModel.P1Id);
            var p2Url = await GetPlayerUrl(apiModel.P2Id);
            Game game = GetGame(gamestate, p1Url, p2Url, apiModel.Seed);
            HttpContext.Current.Cache.Add(gamestate.GameId, game, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 10, 0), CacheItemPriority.High, null);
            return Ok(gamestate);
        }

        private async Task<string> GetPlayerUrl(int id)
        {
            if (id > 0)
            {
                var bot = await _db.PlayerBots.FirstOrDefaultAsync(x => x.Id == id);
                return bot.URL;
            }
            return "";
        }

        private static Game GetGame(GameState gamestate, string side1Url, string side2Url, int seed)
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

        [Route("api/updategame")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGameState(UpdateGameApiModel apiModel)
        {
            var game = HttpContext.Current.Cache[apiModel.GameState.GameId] as Game;
            if (game == null)
            {
                return BadRequest("Could not find that GameState in cache.");
            }
            var p1Moves = await GetPlayerMoves(game, apiModel.GameState, "p1", apiModel.P1Moves);
            var p2Moves = await GetPlayerMoves(game, apiModel.GameState, "p2", apiModel.P2Moves);

            game.UpdateGameState(new[] { p1Moves, p2Moves });
            return Ok(game.GameState);
        }

        private static async Task<PlayerMoves> GetPlayerMoves(Game game, GameState gameState, string pName, BotletMove[] existingMoves)
        {
            if (existingMoves != null)
            {
                return CreatePlayerMoves(pName, existingMoves);
            }
            return await FetchPlayerMoves(game, gameState, pName);
        }

        private static async Task<PlayerMoves> FetchPlayerMoves(Game game, GameState gameState, string pName)
        {
            var player = game.Players.First(x => x.PlayerName.ToLower() == pName);
            var playerMoves = await BattleController.GetPlayerMovesAsync(player, gameState);
            return playerMoves;
        }

        private static PlayerMoves CreatePlayerMoves(string pName, BotletMove[] existingMoves)
        {
            var moves = new PlayerMoves() { Moves = existingMoves, PlayerName = pName };
            return moves;
        }
    }

    public class BotIdApiModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class StartGameApiModel
    {
        [JsonProperty("p1Id")]
        public int P1Id { get; set; }

        [JsonProperty("p2Id")]
        public int P2Id { get; set; }

        [JsonProperty("seed")]
        public int Seed { get; set; }
    }

    public class UpdateGameApiModel
    {
        [JsonProperty("gameState")]
        public GameState GameState { get; set; }

        [JsonProperty("p1Moves")]
        public BotletMove[] P1Moves { get; set; }

        [JsonProperty("p2Moves")]
        public BotletMove[] P2Moves { get; set; }
    }
}
