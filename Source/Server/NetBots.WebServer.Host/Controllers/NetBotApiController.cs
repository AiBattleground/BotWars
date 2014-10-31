using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NetBots.Web;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Host.Controllers;

namespace NetBotsHostProject.Controllers
{
    public class NetBotApiController : ApiController
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        public async Task<IHttpActionResult> GetBotList()
        {
            //todo: Get only visible ones once the private bots branch is merged in.
            var visibleBots = await _db.PlayerBots.ToListAsync();
            var returnObjects = visibleBots.Select(x => new {x.Id, x.Name});
            return Ok(returnObjects);
        }

        public async Task<IHttpActionResult> GetStartingGame(string opponentUrl, string side)
        {
            var battleController = new BattleController();
            var game = battleController.GetNewGameState();
            throw new NotImplementedException();
        } 
    }
}
