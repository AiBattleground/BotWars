using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBots.WebServer.Model;

namespace NetBots.WebServer.Host.Models
{
    public class BattleViewModel
    {
        public PlayerBot Bot1 { get; set; }
        public PlayerBot Bot2 { get; set; }
    }
}