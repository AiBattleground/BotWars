using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBots.WebServer.Model;

namespace NetBots.WebServer.Host.Models
{
    public class PlayerBotViewModel
    {
        public PlayerBotViewModel() { }

        public PlayerBotViewModel(PlayerBot bot)
        {
            this.Id = bot.Id;
            this.Image = bot.Image;
            this.Name = bot.Name;
            this.Owner = bot.Owner.UserName;
            this.OwnerId = bot.Owner.Id;
            this.Private = bot.Private;
            this.Url = bot.URL;
        }

        public int Id { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string OwnerId { get; set; }
        public int Rank { get; set; }
        public bool OwnedByUser { get; set; }
        public bool Private { get; set; }
        public bool Delete { get; set; }
        public string Url { get; set; }

        public List<GameSummary> MatchHistory { get; set; } 


    }
}