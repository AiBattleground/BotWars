using System.ComponentModel.DataAnnotations;

namespace NetBots.WebServer.Model
{
    public class PlayerBot
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string URL { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }
}
