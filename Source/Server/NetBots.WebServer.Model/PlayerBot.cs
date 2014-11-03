using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetBots.WebServer.Model
{
    public class PlayerBot
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public byte[] Image { get; set; }
        public bool Private { get; set; }

        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        
    }
}
