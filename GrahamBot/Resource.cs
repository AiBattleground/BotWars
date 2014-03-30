using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrahamBot
{
    public enum Resource
    {
        Energy = '*',
        RedBotlet = 'r',
        BlueBotlet = 'b',
        EmptySpace = '.',
        DeadRedBotlet = 'x',
        DeadBlueBotlet = 'X',
        RedSpawn = '1',
        BlueSpawn = '2'
    }
}