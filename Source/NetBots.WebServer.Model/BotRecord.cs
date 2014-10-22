using System.ComponentModel.DataAnnotations;

namespace NetBots.WebServer.Model
{
    public class BotRecord
    {
        [Key]
        public int Id { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }
}
