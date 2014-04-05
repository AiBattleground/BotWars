using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BotWars.Core;

namespace GrahamBot
{
    public class BotPlayer: Player
    {
        public string stateName { get; set; }
        public string color { get; set; }
        public string dataName { get; set; }
        public Resource resource { get; set; }
    }
}