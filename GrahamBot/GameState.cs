using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrahamBot
{
    public class GameState
    {
        private Player player1;
        private Player player2;
        public int rows { get; set; }
        public int cols { get; set; }
        public string grid { get; set; }
        public int maxTurns { get; set; }
        public int turnsElapsed { get; set; }
        public Player p1
        {
            get
            {
                return player1;
            }
            set
            {
                if (player1 == null) { player1 = new Player(); }
                player1.color = "red";
                player1.stateName = "p1";
                player1.dataName = "r";
                player1.energy = value.energy;
                player1.spawn = value.spawn;
                player1.resource = Resource.RedBotlet;
            }
        }   
        public Player p2
        {
            get
            {
                return player2;
            }
            set
            {
                if (player2 == null) { player2 = new Player(); }
                player2.color = "blue";
                player2.stateName = "p2";
                player2.dataName = "b";
                player2.energy = value.energy;
                player2.spawn = value.spawn;
                player2.resource = Resource.BlueBotlet;
            }
        }
    }
}