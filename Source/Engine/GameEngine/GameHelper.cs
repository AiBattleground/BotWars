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
            while (sb.Length < 20)
            {
                //a-z are unicode 97 through 122. 
                var randomNumber = myRandom.Next(87, 123);
                if (randomNumber < 97)
                {
                    var oneThroughTen = randomNumber - 87;
                    sb.Append(oneThroughTen);
                }
                else
                {
                    var myChar = (Char)randomNumber;
                    bool capitalize = myRandom.Next(0, 2) == 0;
                    if (capitalize)
                    {
                        myChar = myChar.ToString().ToUpper()[0];
                    }
                    sb.Append(myChar);
                }
            }
            var gameId = sb.ToString();
            return gameId;
        }
    }
}
