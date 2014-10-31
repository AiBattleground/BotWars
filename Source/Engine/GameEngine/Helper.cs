using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBots.GameEngine
{
    public static class GameHelper
    {
        public static string GenerateRandomGameId()
        {
            Random myRandom = new Random();
            var sb = new StringBuilder();
            while (sb.Length < 8)
            {
                //a-z are unicode 97 through 122. 
                var randomNumber = myRandom.Next(97, 123);
                var myChar = (Char)randomNumber;
                sb.Append(myChar);
            }
            var gameId = sb.ToString();
            return gameId;
        }
    }
}
