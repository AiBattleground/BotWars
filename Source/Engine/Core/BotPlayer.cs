﻿using NetBots.Bot.Interface;

namespace NetBots.Core
{
    public class BotPlayer : Player
    {
        public string PlayerName { get; set; }
        public char BotletId { get; set; }
        public Resource Resource { get; set; }
        public char deadBotletId { get; set; }
    }
}