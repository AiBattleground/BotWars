﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetBots.Bot.Interface;
using NetBots.Core;

namespace NetBots.GameEngine
{
    public class Game
    {
        private readonly IDice _myDice;
        public GameState GameState;
        public List<BotPlayer> Players;
        int _energySpawnFrequency;

        //this is creating a schism between where the player should be updated. need gamestate with more dynamic players
        public Game(GameState gameState, IEnumerable<BotPlayer> players){
            GameState = gameState;
            _myDice = new Dice();
            Players = players.ToList();
            _energySpawnFrequency = 5;
        }

        public Game(GameState gameState, IEnumerable<BotPlayer> players, int seed)
        {
            GameState = gameState;
            _myDice = new Dice(seed);
            Players = players.ToList();
            _energySpawnFrequency = 5;
        }

        public Game(GameState gameState, IEnumerable<BotPlayer> players, IDice dice)
        {
            GameState = gameState;
            _myDice = dice;
            Players = players.ToList();
            _energySpawnFrequency = 5;
        }

        public void UpdateGameState(IEnumerable<PlayerMoves> playersMoves)
        {
            _ClearDeadBotlets();
            _ApplyMoves(playersMoves);
            _FightBattles();
            _RazeBases();
            _SpawnBots();
            _CollectEnergy();
            _PlaceEnergy();
            _UpdateTurns();
            _CheckForWinner();
        }

        private void _UpdateTurns()
        {
            GameState.TurnsElapsed++;
        }

        private void _ClearDeadBotlets()
        {
            Players.ToList().ForEach(p => GameState.Grid = GameState.Grid.Replace(p.deadBotletId, '.'));
        }

        private void _ApplyMoves(IEnumerable<PlayerMoves> playersMoves)
        {
            _SanitizeMoves(playersMoves);
            _removeBotletsFromOldLocations(playersMoves);
            _placeBotletsOnNewLocations(playersMoves);
        }

        private void _RazeBases()
        {
            foreach(BotPlayer botPlayer in Players){
                Player player = _GetPlayer(botPlayer);
                if (player.spawnDisabled != true && _GetEnemyBotletIds(botPlayer.BotletId).Contains(GameState.Grid[player.spawn]))
                {
                    player.spawnDisabled = true;
                }
            }
        }

        private void _CollectEnergy()
        {           
            List<int> energyLocations = _GetEnergyLocations();
            foreach (int energyLocation in energyLocations)
            {
                _AssignEnergyToBot(energyLocation);
            }
        }

        private void _AssignEnergyToBot(int location)
        {
            BotPlayer winningPlayer = _GetEnergyWinningPlayer(location);
            if (winningPlayer != null)
            {
                StringBuilder grid = new StringBuilder(GameState.Grid);
                grid[location] = '.';
                GameState.Grid = grid.ToString();
                _GetPlayer(winningPlayer).energy++;
            }
        }

        private BotPlayer _GetEnergyWinningPlayer(int location)
        {
            List<int> adjacentPositions = new SpatialGameState(GameState).AdjacentPositions(location).ToList();
            BotPlayer winningPlayer = null;
            int botsAdjacentToEnergy = 0;
            foreach (BotPlayer player in Players)
            {
                int playersBotsAdjacentToEnergy = adjacentPositions
                    .Where(a => GameState.Grid[a] == player.BotletId)
                    .Count();
                if (playersBotsAdjacentToEnergy > botsAdjacentToEnergy)
                    winningPlayer = player;
                else if (playersBotsAdjacentToEnergy == botsAdjacentToEnergy)
                    winningPlayer = null;
                botsAdjacentToEnergy = Math.Max(botsAdjacentToEnergy, playersBotsAdjacentToEnergy);
            }

            return winningPlayer;
        }

        private List<int> _GetEnergyLocations()
        {
            List<int> energyLocations = new List<int>();
            for(int i = 0; i< GameState.Grid.Length; i++){
                if(GameState.Grid[i] == '*') energyLocations.Add(i);
            }
            return energyLocations;
        }

        private void _FightBattles()
        {
            IEnumerable<Botlet> botlets = _GetBotlets();
            List<Botlet> losingBotlets = new List<Botlet>();
            foreach (Botlet botlet in botlets)
            {
                IEnumerable<Botlet> adjacentEnemies = _GetAdjacentEnemies(botlet);
                foreach (Botlet adjacentEnemy in adjacentEnemies)
                {
                    if (adjacentEnemies.Count() >= _GetAdjacentEnemies(adjacentEnemy).Count())
                        losingBotlets.Add(botlet);
                }
            }
            _KillLosingBotlets(losingBotlets);
        }

        private void _KillLosingBotlets(List<Botlet> losingBotlets)
        {
            StringBuilder grid = new StringBuilder(GameState.Grid);
            losingBotlets.ForEach(lb => grid[lb.gridPosition] = Players.Find(p=>p.BotletId==lb.botletId).deadBotletId);
            GameState.Grid = grid.ToString();
        }

        private IEnumerable<Botlet> _GetAdjacentEnemies(Botlet botlet)
        {
            IEnumerable<char> enemyBotletIds = _GetEnemyBotletIds(botlet.botletId);
            IEnumerable<Botlet> EnemyBotlets = _GetBotlets().Where(b => enemyBotletIds.Contains(b.botletId));
            IEnumerable<int> adjacentPositions = new SpatialGameState(GameState).AdjacentPositions(botlet.gridPosition);
            return EnemyBotlets.Where(b => adjacentPositions.Contains(b.gridPosition));
        }

        private IEnumerable<char> _GetEnemyBotletIds(char playerBotletId)
        {
            return Players.Select(p => p.BotletId).Where(c => c != playerBotletId);
        }

        private IEnumerable<Botlet> _GetBotlets()
        {
           return GameState.Grid
                .Select((c, i) => new Botlet(){ botletId = c, gridPosition = i})
                .Where(b=>Players.Select(p=>p.BotletId).Contains(b.botletId));
        }

        private void _SpawnBots()
        {
            Players.ToList().ForEach(p => _SpawnBot(p));
        }

        private void _SpawnBot(BotPlayer botPlayer){
            Player player = _GetPlayer(botPlayer);
            if (player.spawnDisabled == false && player.energy > 0 && GameState.Grid[player.spawn] == '.')
            {
                StringBuilder grid = new StringBuilder(GameState.Grid);
                grid[player.spawn] = botPlayer.BotletId;
                GameState.Grid = grid.ToString();
                player.energy--;
                //_GetPlayer(player).energy = player.energy;
            }
        }

        private Player _GetPlayer(BotPlayer player)
        {
            Dictionary<string, Player> players = new Dictionary<string, Player>() 
                { {"p1", GameState.P1}, {"p2", GameState.P2} };
            return players[player.PlayerName];
        }

        private void _PlaceEnergy()
        {
            List<Tuple<int,int>> symetricEmptySpaces =  _GetSymetricEmptySpaces();
            if (GameState.TurnsElapsed % _energySpawnFrequency == 0 && symetricEmptySpaces.Count > 0)
            {
                Tuple<int, int> emptySpaces = _GetRandomPairOfEmptySpaces(symetricEmptySpaces);
                _PlaceEnergy(emptySpaces);
            }
        }

        private void _PlaceEnergy(Tuple<int, int> emptySpaces)
        {
            StringBuilder grid = new StringBuilder(GameState.Grid);
            grid[emptySpaces.Item1] = '*';
            grid[emptySpaces.Item2] = '*';
            GameState.Grid = grid.ToString();
        }

        private Tuple<int, int> _GetRandomPairOfEmptySpaces(List<Tuple<int, int>> symetricEmptySpaces)
        {
            var spaces = symetricEmptySpaces[_myDice.Next(symetricEmptySpaces.Count)];
            return spaces;
        }

        private List<Tuple<int, int>> _GetSymetricEmptySpaces()
        {
            List<Tuple<int, int>> emptySpaces = new List<Tuple<int, int>>();
            for (int i = 0; i < GameState.Grid.Length; i++)
            {
                int first = i;
                int second = GameState.Grid.Length - 1 - i;
                if (GameState.Grid[first] == '.' && GameState.Grid[second] == '.')
                {
                    emptySpaces.Add(new Tuple<int,int>(first,second));
                }
            }
            return emptySpaces;
        }

        private void _CheckForWinner()
        {
            //AreMovesWellFormed
        }

        private void _SanitizeMoves(IEnumerable<PlayerMoves> playersMoves)
        {
            foreach(PlayerMoves playerMoves in playersMoves){
                BotPlayer player = Players.Find(p => p.PlayerName == playerMoves.PlayerName);
                if (playerMoves.Moves == null)
                {
                    playerMoves.Moves = new List<BotletMove>();
                }
                else
                {
                    playerMoves.Moves = _GetValidMovesOnly(playerMoves.Moves, player.BotletId);
                }
            }
        }

        private IEnumerable<BotletMove> _GetValidMovesOnly(IEnumerable<BotletMove> moves, char botletId)
        {
            List<BotletMove> validMoves = new List<BotletMove>();
            foreach(BotletMove move in moves){
                List<bool> requirements = new List<bool>();
                requirements.Add(GameState.Grid[move.From] == botletId);
                requirements.Add(!validMoves.Select(m=>m.From).Contains(move.From));
                requirements.Add(move.To >=0 && move.To<GameState.Grid.Length);
                if (requirements.All(r=>r==true))
                {
                    validMoves.Add(move);
                }
            }

            return validMoves;
        }

        private bool _AreMovesWellFormed(IEnumerable<BotletMove> moves)
        {
            return true;
        }

        private void _removeBotletsFromOldLocations(IEnumerable<PlayerMoves> playersMoves)
        {
            IEnumerable<BotletMove> moves = playersMoves.Aggregate(new List<BotletMove>(), (sum, pm) => sum.Concat(pm.Moves).ToList());
            char[] grid = GameState.Grid.ToCharArray();
            foreach(BotletMove move in moves)
            {
                grid.SetValue('.', move.From);
            }
            GameState.Grid = new string(grid);
        }

        private void _placeBotletsOnNewLocations(IEnumerable<PlayerMoves> playersMoves)
        {
            char[] grid = GameState.Grid.ToCharArray();
            foreach (PlayerMoves playerMoves in playersMoves)
            {
                BotPlayer botPlayer = Players.Find(p => p.PlayerName == playerMoves.PlayerName);
                foreach (BotletMove move in playerMoves.Moves)
                {
                    switch (grid[move.To])
                    {
                        //this rule seems weird, it implies that you can collect/spawn on same turn.
                        case  '*':
                            if (botPlayer.PlayerName == "p1")
                                GameState.P1.energy++;
                            else
                                GameState.P2.energy++;
                            break;
                        case '.':
                            grid[move.To] = botPlayer.BotletId;
                            break;
                        default:
                            grid[move.To] = botPlayer.deadBotletId;
                            break;
                    }
                }
            }
            GameState.Grid = new string(grid);
        }

        private char[] _GetGridAfterMove(char[] grid, string player){
            return grid;
        }
    }
}
