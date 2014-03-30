using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrahamBot
{
    public class MoveRequest
    {
        public GameState state { get; set; }
        public string player { get; set; }
    }
}