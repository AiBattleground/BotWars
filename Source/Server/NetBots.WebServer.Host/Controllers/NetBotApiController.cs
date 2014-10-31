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
using NetBots.GameEngine;
using NetBots.Web;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Host.Controllers;

namespace NetBotsHostProject.Controllers
{
    public class NetBotApiController : ApiController
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();

        public async Task<IHttpActionResult> GetBotList()
        {
            //todo: Get only visible ones once the private bots branch is merged in.
            var visibleBots = await _db.PlayerBots.ToListAsync();
            var returnObjects = visibleBots.Select(x => new BotId() { Name = x.Name, Id = x.Id });
            return Ok(returnObjects);
        }

        public async Task<IHttpActionResult> GetStartingGame(int opponentId, string side)
        {
            var battleController = new BattleController();
            var gamestate = battleController.GetNewGameState();
            var opponent = await _db.PlayerBots.FirstOrDefaultAsync(x => x.Id == opponentId);
            var opponentUrl = opponent.URL;
            var side1Url = side == "p1" ? null : opponentUrl;
            var side2Url = side == "p2" ? null : opponentUrl;
            var players = battleController.GetPlayers(20, side1Url, side2Url);
            var game = new Game(gamestate, players);
            HttpContext.Current.Cache.Add(gamestate.GameId, game, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 10, 0), CacheItemPriority.High, null);
            return Ok(gamestate);
        }

        public async Task<IHttpActionResult> UpdateGameState(GameState gameState, PlayerMoves clientMoves)
        {
            var game = HttpContext.Current.Cache[gameState.GameId] as Game;
            if (game == null)
            {
                return BadRequest("Could not find that GameState in cache.");
            }
            var opponent = game.Players.First(x => x.Uri != null);
            var opponentMoves = await BattleController.GetPlayerMovesAsync(opponent, gameState);
            game.UpdateGameState(new[] { clientMoves, opponentMoves });
            return Ok(game.GameState);
        }
    }

    public class BotId
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
