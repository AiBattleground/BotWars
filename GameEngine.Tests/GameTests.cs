using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BotWars.Core;
using BotWars.GameEngine;
using System.Collections.Generic;
using Moq;
using NetBots.Bot.Interface;
using Newtonsoft.Json;

namespace GameEngine.Tests
{
    [TestClass]
    public class GameTests
    {
        private GameState _GetNewGameState(int boardWidth, int startingEnergy = 0)
        {
            return new GameState()
            {
                rows = boardWidth,
                cols = boardWidth,
                p1 = new Player() { energy = startingEnergy, spawn = 0, spawnDisabled = false},
                p2 = new Player() { energy = startingEnergy, spawn = ((boardWidth * 2) - 1), spawnDisabled = false},
                grid = new string('.', boardWidth * boardWidth),
                maxTurns = 200,
                turnsElapsed = 0

            };
        }

        public BotPlayer GetBotPlayer(bool red)
        {
            var botPlayer = new BotPlayer()
            {
                botletId = red ? 'r' : 'b',
                color = red ? "red" : "blue",
                deadBotletId = 'x',
                playerName = red ? "p1" : "p2"
            };
            return botPlayer;
        }


        [TestMethod]
        public void GetGameStateBackFromGetNextGameState()
        {
            //Game game = new Game(_GetNewGameState(20));
            //List<BotletMove> redMoves = null;// new List<BotletMove>();
            //List<BotletMove> blueMoves = new List<BotletMove>();
            //game.UpdateGameState(redMoves, blueMoves);
            //Assert.IsInstanceOfType(game.GameState, typeof(GameState));
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

        [TestMethod]
        public void CorrectFightResolution()
        {
            var gs = _GetNewGameState(3, 0);
            string grid =
                "b.." +
                "..." +
                "r.b";
            gs.grid = grid;
            
            var game = new Game(gs, new[]{new BotPlayer()
            {
                botletId = 'r',
                color = "red",
                deadBotletId = 'x',
                energy = '0',
                playerName = "p1"
            }, new BotPlayer()
            {
                botletId = 'b',
                color = "blue",
                deadBotletId = 'x',
                energy = '0',
                playerName = "p2"
            }});
            var playerMoves = new PlayerMoves()
            {
                PlayerName = "p2",
                Moves = new[] {new BotletMove(0, 3), new BotletMove(8, 7)}
            };
            game.UpdateGameState(new[]{playerMoves});
            Assert.IsTrue(game.GameState.grid.Count(x => x == 'r') == 0);
            Assert.IsTrue(game.GameState.grid.Count(x => x == 'b') == 2);
        }

        [TestMethod]
        public void Issue6_EnergyPreventsSpawn()
        {
            const int diceReuslt = 0;
            var loadedDice = new Mock<IDice>();
            loadedDice.Setup(x => x.Next()).Returns(diceReuslt);
            loadedDice.Setup(x => x.Next(It.IsAny<int>())).Returns(diceReuslt);
            loadedDice.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(diceReuslt);

            var gs = _GetNewGameState(3, 0);
            string grid =
                "..." +
                "..." +
                ".r.";
            gs.grid = grid;

            var game = new Game(gs, new[]{ GetBotPlayer(true), GetBotPlayer(false)}, loadedDice.Object);
            game.GameState.p1.energy = 0;
            Assert.IsTrue(game.GameState.grid.Count(x => x == '*') == 0);
            Assert.IsTrue(game.GameState.grid.Count(x => x == 'r') == 1);
            game.UpdateGameState(new List<PlayerMoves>());
            game.UpdateGameState(new List<PlayerMoves>());
            Assert.IsTrue(game.GameState.grid.Count(x => x == 'r') == 2);
        }
    }
}
