using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotWars.Core;
using BotWars.GameEngine;
using System.Collections.Generic;

namespace GameEngine.Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void GetGameStateBackFromGetNextGameState()
        {
            int boardWidth = 20;
            GameState newGameState = new GameState()
            {
                rows = boardWidth,
                cols = boardWidth,
                p1 = new Player() { energy = 1, spawn = boardWidth + 1 },
                p2 = new Player() { energy = 1, spawn = boardWidth * (boardWidth - 1) - 2 },
                grid = new string('.', boardWidth * boardWidth),
                maxTurns = 200,
                turnsElapsed = 0
            };
            Game game = new Game();
            List<BotletMove> redMoves = new List<BotletMove>();
            List<BotletMove> blueMoves = new List<BotletMove>();
            GameState nextGameStatae = Game.GetNextGameState(newGameState,redMoves,blueMoves);
            Assert.IsInstanceOfType(nextGameStatae, typeof(GameState));
        }
    }
}
