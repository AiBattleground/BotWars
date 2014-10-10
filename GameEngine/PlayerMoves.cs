﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotWars.Core;
using NetBots.Bot.Interface;

namespace BotWars.GameEngine
{
    public class PlayerMoves
    {
        public string PlayerName { get; set; }
        public IEnumerable<BotletMove> Moves { get; set; }
    }
}
