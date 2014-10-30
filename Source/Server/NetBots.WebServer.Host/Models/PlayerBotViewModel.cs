using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBots.WebServer.Model;

namespace NetBotsHostProject.Models
{
    public class PlayerBotViewModel
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Rank { get; set; }
        public bool OwnedByUser { get; set; }

        public List<GameSummary> MatchHistory { get; set; } 
    }
}