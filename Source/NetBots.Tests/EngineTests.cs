using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetBots.Core;
using NetBots.GameEngine;
using NetBots.WebModels;
using NetBotsHostProject.Controllers;
using Newtonsoft.Json;

namespace NetBots.Tests
{
    [TestClass]
    public class EngineTests
    {
        [TestMethod]
        public void BotsCanMove()
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
            game.EnergySpawnFrequency = 1000;
            var p1moves = new PlayerMoves()
            {
                PlayerName = "p1",
                Moves = new[] { new BotletMove(0, 1) }
            };
            game.UpdateGameState(new[] { p1moves });
            var grid = gameState.Grid;
            Assert.IsTrue(grid.IndexOf('1') == 1);

            p1moves.Moves = new[] {new BotletMove(1, 7)};
            game.UpdateGameState(new[] {p1moves});
            Assert.IsTrue(gameState.Grid.IndexOf('1') == 7);

            p1moves.Moves = new[] { new BotletMove(7, 6) };
            game.UpdateGameState(new[] { p1moves });
            Assert.IsTrue(gameState.Grid.IndexOf('1') == 6);

            p1moves.Moves = new[] { new BotletMove(6, 0) };
            game.UpdateGameState(new[] { p1moves });
            Assert.IsTrue(gameState.Grid.IndexOf('1') == 0);
        }

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

        [TestMethod]
        public void Issue18_Teleport()
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
                Moves = new[] { new BotletMove(0, gameState.Grid.Length - 1) }
            };
            game.UpdateGameState(new[] { p1moves });
            var grid = gameState.Grid;
            Assert.IsTrue(grid.IndexOf('1') != gameState.Grid.Length - 1);
        }

        [TestMethod]
        public void EnergyNeverCollected()
        {
            var gameState = new GameState();
            gameState.Cols = 6;
            gameState.Rows = 6;
            gameState.MaxTurns = 5;
            gameState.TurnsElapsed = 1;
            gameState.Grid = "1....." +
                             "*2...." +
                             "......" +
                             "......" +
                             "......" +
                             "......";
            gameState.P1 = new Player() { Spawn = 0 };
            gameState.P2 = new Player() { Spawn = gameState.Grid.Length - 1 };
            var game = new Game(gameState, "", "");
            game.EnergySpawnFrequency = 100000;
           
            var p1moves = new PlayerMoves()
            {
                PlayerName = "p1",
                Moves = new[] { new BotletMove(0, 0) }
            };
            for (int i = 0; i < 200; i++)
            {
                game.UpdateGameState(new[] { p1moves });
            }
            var grid = gameState.Grid;
            Assert.IsTrue(grid.IndexOf('1') == 0);
            Assert.IsTrue(grid.IndexOf('2') == 7);
            Assert.IsTrue(grid.IndexOf('*') == 6);
        }

        [TestMethod]
        public void CanDeserializeJson()
        {
            var json = "{\"gameState\":{\"gameId\":\"8eZDZ7u1cuAn987E1856\",\"apiKey\":null,\"secretKey\":null,\"rows\":20,\"cols\":20,\"p1\":{\"energy\":1,\"spawn\":21,\"spawnDisabled\":false},\"p2\":{\"energy\":1,\"spawn\":378,\"spawnDisabled\":false},\"grid\":\"................................................................................................................................................................................................................................................................................................................................................................................................................\",\"maxTurns\":200,\"turnsElapsed\":0,\"winner\":null},\"p1Moves\":null,\"p2Moves\":null}";
            var moveReq = JsonConvert.DeserializeObject<UpdateGameApiModel>(json);
            Assert.IsTrue(moveReq != null);
        }
    }
}
 