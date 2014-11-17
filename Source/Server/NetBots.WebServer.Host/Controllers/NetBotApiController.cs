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
using NetBots.GameEngine;
using NetBots.Web;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Host.Controllers;
using NetBots.WebServer.Host.Models;
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
        public async Task<IHttpActionResult> StartGame(StartGameApiModel startGameModel)
        {
            var battleController = new BattleController();
            var gamestate = battleController.GetNewGameState();
            var opponent = await _db.PlayerBots.FirstOrDefaultAsync(x => x.Id == startGameModel.OpponentId);
            var opponentUrl = opponent.URL;
            var side1Url = startGameModel.Side.ToLower() == "p1" ? "" : opponentUrl;
            var side2Url = startGameModel.Side.ToLower() == "p2" ? "" : opponentUrl;
            Game game = new Game(gamestate, side1Url, side2Url, startGameModel.Seed);
            HttpContext.Current.Cache.Add(gamestate.GameId, game, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 10, 0), CacheItemPriority.High, null);
            return Ok(gamestate);
        }

        [Route("api/updategame")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGameState(UpdateGameApiModel updateGameModel)
        {
            var game = HttpContext.Current.Cache[updateGameModel.GameState.GameId] as Game;
            if (game == null)
            {
                return BadRequest("Could not find that GameState in cache.");
            }
            var opponent = game.Players.First(x => x.Uri != null && x.Uri.Replace("http://", "") != "");
            var opponentMoves = await BattleController.GetPlayerMovesAsync(opponent, updateGameModel.GameState);
            var clientMoves = new PlayerMoves() {Moves = updateGameModel.ClientMoves, PlayerName = opponentMoves.PlayerName.ToLower() == "p1" ? "p2" : "p1"};
            game.UpdateGameState(new[] { clientMoves, opponentMoves });
            return Ok(game.GameState);
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
        [JsonProperty("opponentId")]
        public int OpponentId { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("seed")]
        public int Seed { get; set; }
    }

    public class UpdateGameApiModel
    {
        [JsonProperty("gameState")]
        public GameState GameState { get; set; }

        [JsonProperty("clientMoves")]
        public BotletMove[] ClientMoves { get; set; }
    }
}
