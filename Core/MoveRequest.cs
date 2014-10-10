using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBots.Bot.Interface;

namespace BotWars.Core
{
    public class MoveRequest
    {
        public GameState state { get; set; }
        public string player { get; set; }
    }
}
