using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotWars.Core;
using BotWars.GameEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameEngine.Tests
{
    [TestClass]
    public class GameTests
    {
        private GameState _GetNewGameState(int boardWidth)
        {
            return new GameState()
            {
                rows = boardWidth,
                cols = boardWidth,
                p1 = new Player() { energy = 1, spawn = boardWidth + 1 },
                p2 = new Player() { energy = 1, spawn = boardWidth * (boardWidth - 1) - 2 },
                grid = new string('.', boardWidth * boardWidth),
                maxTurns = 200,
                turnsElapsed = 0
            };
        }
        [TestMethod]
        public void GetGameStateBackFromGetNextGameState()
        {
            Game game = new Game(_GetNewGameState(20));
            List<BotletMove> redMoves = null;// new List<BotletMove>();
            List<BotletMove> blueMoves = new List<BotletMove>();
            game.UpdateGameState(redMoves, blueMoves);
            Assert.IsInstanceOfType(game.GameState, typeof(GameState));
        }
        [TestMethod]
        public void CanAssignWinner()
        {
            string jsonGameState = "{\"rows\":20,\"cols\":20,\"p1\":{\"energy\":1,\"spawn\":21},\"p2\":{\"energy\":1,\"spawn\":378},\"grid\":\"................................................................................................................................................................................................................................................................................................................................................................................................................\",\"maxTurns\":200,\"turnsElapsed\":0,\"winner\":\"b\"}";
            GameState newGameState = _GetNewGameState(20);
            newGameState.winner = "b";
            Assert.AreEqual(jsonGameState, JsonConvert.SerializeObject(newGameState));
        }
        [TestMethod]
        public void NoWinnerIgnoresWinnerProperty()
        {
            string jsonGameState = "{\"rows\":20,\"cols\":20,\"p1\":{\"energy\":1,\"spawn\":21},\"p2\":{\"energy\":1,\"spawn\":378},\"grid\":\"................................................................................................................................................................................................................................................................................................................................................................................................................\",\"maxTurns\":200,\"turnsElapsed\":0}";
            GameState newGameState = _GetNewGameState(20);
            Assert.AreEqual(jsonGameState, JsonConvert.SerializeObject(newGameState));
        }
        [TestMethod]
        public void TestMethodScratchpad()
        {
            string grid = "x.......x";
            char[] gridCharArray = grid.ToCharArray();
            gridCharArray.SetValue('.', 0);
            grid = new string(gridCharArray);
            Assert.AreEqual("x.......x", grid);
        }
    }
}
