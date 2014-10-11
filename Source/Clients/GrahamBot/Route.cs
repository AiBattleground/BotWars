using NetBots.Core;

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
