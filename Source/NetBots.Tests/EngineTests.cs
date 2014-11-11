using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetBots.Core;
using NetBots.GameEngine;
using NetBots.Web;

namespace NetBots.Tests
{
    [TestClass]
    public class EngineTests
    {
        [TestMethod]
        public void EnergyBug()
        {
            var gameState = new GameState();
            gameState.Cols = 6;
            gameState.Rows = 6;
            gameState.MaxTurns = 5;
            gameState.TurnsElapsed = 0;
            gameState.Grid = "......" +
                             ".1...." +
                             ".*2..." +
                             "......" +
                             "......" +
                             "......";
            gameState.P1 = new Player(){Spawn = 0};
            gameState.P2 = new Player(){Spawn = gameState.Grid.Length - 1};
            var game = new Game(gameState, "", ""); 
            var p1moves = new PlayerMoves() 
            {
                PlayerName = "p1", 
                Moves = new[]  {new BotletMove(gameState.Grid.IndexOf('1'), gameState.Grid.IndexOf('*'))}
            };
            var p2moves = new PlayerMoves()
            {
                PlayerName = "p2",
                Moves = new[] { new BotletMove(gameState.Grid.IndexOf('2'), gameState.Grid.IndexOf('*')) }
            };
            game.UpdateGameState(new[] {p1moves, p2moves});
            var grid = gameState.Grid;
            Assert.IsTrue(grid.Count(x => x == '1') == grid.Count(x => x == '2'));
            Assert.IsTrue(gameState.P1.Energy == gameState.P2.Energy);
        }


        [TestMethod]
        public void Issue18_DiagonalMoveBug()
        {
            var gameState = new GameState();
            gameState.Cols = 6;
            gameState.Rows = 6;
            gameState.MaxTurns = 5;
            gameState.TurnsElapsed = 0;
            gameState.Grid = "1....." +
                             "......" +
                             "......" +
                             "......" +
                             "......" +
                             "......";
            gameState.P1 = new Player() { Spawn = 0 };
            gameState.P2 = new Player() { Spawn = gameState.Grid.Length - 1 };
            var game = new Game(gameState, "", "");
            var p1moves = new PlayerMoves()
            {
                PlayerName = "p1",
                Moves = new[] { new BotletMove(0, 7) }
            };
            game.UpdateGameState(new[] { p1moves });
            var grid = gameState.Grid;
            Assert.IsTrue(grid.IndexOf('1') != 7);
        }
    }
}
 