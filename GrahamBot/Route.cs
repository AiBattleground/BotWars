using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotWars.Core;

namespace GrahamBot
{
    class Route
    {
        public Route(Space start, Space end)
        {
            Start = start;
            End = end;
        }
        public Space Start { get; set; }
        public Space End { get; set; }
    }
}
