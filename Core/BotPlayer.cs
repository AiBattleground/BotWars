using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBots.Bot.Interface;

namespace BotWars.Core
{
    public class BotPlayer : Player
    {
        public string playerName { get; set; }
        public string color { get; set; }
        public char botletId { get; set; }
        public Resource resource { get; set; }
        public char deadBotletId { get; set; }
    }
}