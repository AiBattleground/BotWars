using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Cors;
using NetBots.GameEngine;
using NetBots.WebModels;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Host.Controllers;
using NetBotsHostProject.Helpers;
using NetBotsHostProject.Hubs;
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
            var gameManager = new WarGameManager();
            var gamestate = gameManager.GetNewGameState();
            var p1Url = await GetPlayerUrl(apiModel.P1Id, gameManager);
            var p2Url = await GetPlayerUrl(apiModel.P2Id, gameManager);
            Game game = gameManager.GetGame(gamestate, p1Url, p2Url, apiModel.Seed);
            HttpContext.Current.Cache.Add(gamestate.GameId, game, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 10, 0), CacheItemPriority.High, null);
            return Ok(gamestate);
        }

        private async Task<string> GetPlayerUrl(int id, WarGameManager manager)
        {
            var bot = await manager.GetPlayerBot(id);
            if (bot != null)
            {
                return bot.URL;
            }
            return "";
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
            if (game.GameState.Winner != null)
            {
                await GetPlayerMoves(game, game.GameState, "p1", apiModel.P1Moves);
                await GetPlayerMoves(game, game.GameState, "p2", apiModel.P2Moves);
            }
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
            var player = game.Players.First(x => x.PlayerName.ToLower() == pName.ToLower());
            var httpMoves = await WarGameManager.GetPlayerMovesAsync(player, gameState);
            return httpMoves.PlayerMoves;
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
