using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotWars.GameEngine
{
    public interface IDice
    {
        int Next();
        int Next(int maxValue);
        int Next(int minValue, int maxValue);
    }

    internal class Dice : IDice
    {
        private readonly Random _myRandom;

        public int Next()
        {
            return _myRandom.Next();
        }

        public int Next(int maxValue)
        {
            return _myRandom.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return _myRandom.Next(minValue, maxValue);
        }

        public Dice(int seed)
        {
            _myRandom = new Random();
        }

        public Dice()
        {
            _myRandom = new Random();
        }
    }
}
