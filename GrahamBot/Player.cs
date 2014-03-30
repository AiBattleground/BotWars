using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrahamBot
{
    public class Player
    {
        public string stateName { get; set; }
        public string color { get; set; }
        public string dataName { get; set; }
        public int energy { get; set; }
        public int spawn { get; set; }
        public Resource resource { get; set; }
    }
}