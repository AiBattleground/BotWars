using System;
using System.Linq;
using System.Collections.Generic;
using NetBots.Bot.Interface;
using NetBots.Core;
using NetBots.Web;

namespace GrahamBot
{
	public class GrahamAi : INetBot
	{
        public IEnumerable<BotletMove> GetMoves(MoveRequest request)
        {
            Grid grid = new Grid(request.State);
            BotPlayer player = _GetBotPlayer(request.Player, request.State);
            BotPlayer enemy = _GetBotPlayer(request.Player == "p2" ? "1" : "2", request.State);
            //current strategy: For each energy, have the closest bot move toward it. 
            //Remaining bots move toward the enemy's base.
            List<BotletMove> responseMoves = _AssignBotletsToGrabEnergy(grid, player, enemy);
            return responseMoves;
        }

		private static BotPlayer _GetBotPlayer(string requestPlayer, GameState state)
		{
			BotPlayer player = new BotPlayer();
			if (requestPlayer == "p1")
			{
				player.PlayerName = "p1";
				player.BotletId = '1';
				player.Energy = state.P1.Energy;
				player.Spawn = state.P1.Spawn;
				player.Resource = Resource.P1Botlet;
			}
			else{
				player.PlayerName = "p2";
				player.BotletId = '2';
				player.Energy = state.P2.Energy;
				player.Spawn = state.P2.Spawn;
				player.Resource = Resource.P2Botlet;
			}
			return player;
		}
		private static List<BotletMove> _AssignBotletsToGrabEnergy(Grid grid, BotPlayer player, BotPlayer enemy)
		{
			List<Space> myBots = grid.GetSpaces(player.Resource).ToList();
			List<Space> energyOpportunities = grid.GetSpaces(Resource.Energy).ToList();
			List<BotletMove> botMoves = new List<BotletMove>();
			while (myBots.Count > 0 && energyOpportunities.Count > 0)
			{
				Route closestRoute = _GetShortestRoute(myBots, energyOpportunities, grid);
				botMoves.Add(_GetBotMoveAlongRoute(closestRoute, grid, botMoves));
				myBots.Remove(closestRoute.Start);
				energyOpportunities.Remove(closestRoute.End);
			}
			Random randomizer = new Random();
			while (myBots.Count > 0)
			{
				Space nextUp = myBots[randomizer.Next(myBots.Count)];
				botMoves.Add(_GetBotMoveAlongRoute(new Route(nextUp, grid.GetSpace(enemy.Spawn)), grid, botMoves));
				myBots.Remove(nextUp);
			}

			return botMoves;
		}

		private static Route _GetShortestRoute(List<Space> playerBotlets, List<Space> energyOpportunities, Grid grid)
		{
			int shortestDistanceToEnergy = int.MaxValue;
			Space closetBotToEnergy = playerBotlets[0];
			Space closestEnergyToBotlet = energyOpportunities[0];
			foreach (Space energy in energyOpportunities)
			{
				foreach (Space botlet in playerBotlets)
				{
					int distanceApart = Math.Abs(botlet.x - energy.x) + Math.Abs(botlet.y - energy.y);
					if (distanceApart < shortestDistanceToEnergy)
					{
						closestEnergyToBotlet = energy;
						closetBotToEnergy = botlet;
						shortestDistanceToEnergy = distanceApart;
					}
				}
			}
			return new Route(closetBotToEnergy, closestEnergyToBotlet);
		}

		private static BotletMove _GetBotMoveAlongRoute(Route route, Grid grid, List<BotletMove> botletMoves)
		{
			Space spaceToMoveTo = _MoveTowardDestination(route.Start, route.End);
			BotletMove move = grid.GetMove(route.Start, spaceToMoveTo);
			if (botletMoves.Select(bm => bm.To).Contains(move.To))
			//stop, don't go there, you'll suicide if you do.
			{
				List<Space> spacesWhereSomeoneElseIsGoing = botletMoves.Select(m => m.To).Select(m => grid.GetSpace(m)).ToList();
				move = grid.GetMove(route.Start, _lessDestructiveMove(route.Start, spacesWhereSomeoneElseIsGoing, grid));
			}
			return move;
		}

		private static Space _PickARandomSpace(List<Space> spaces)
		{
			Random randomizer = new Random();
			return spaces[randomizer.Next(spaces.Count)];
		}

		private static List<Space> _AdjacentSpaces(Space space)
		{
			return new List<Space>()
			{ 
				new Space() { x = space.x + 1, y = space.y },
				new Space() { x = space.x, y = space.y - 1 },
				new Space() { x = space.x, y = space.y + 1 },
				new Space() { x = space.x - 1, y = space.y }
			};

		}

		private static List<Space> _AllowedSpaces(Space space, Grid grid){
			List<Space> allowedSpaces = _AdjacentSpaces(space).Where(s => _WithinGrid(grid, s)).ToList();
			allowedSpaces.Add(space);
			return allowedSpaces;
		}

		private static bool _WithinGrid(Grid grid, Space space)
		{
			bool xWithinBounds = space.x >= 0 && space.x < grid.First().ToList().Count;
			bool yWithinBounds = space.y >= 0 && space.y < grid.ToList().Count;
			return xWithinBounds && yWithinBounds;
		}

		public static Space _lessDestructiveMove(Space botlet, List<Space> spacesWhereSomeoneElseIsGoing, Grid grid){
			Space lessDestructiveMove;
			List<Space> lessDestructiveMoves = _AllowedSpaces(botlet, grid).Except(spacesWhereSomeoneElseIsGoing).ToList();
			if(lessDestructiveMoves.Count> 0)
				lessDestructiveMove = _PickARandomSpace(lessDestructiveMoves);
			else lessDestructiveMove = botlet;
			return lessDestructiveMove;
		}

		private static Space _MoveTowardDestination(Space bot, Space destination)
		{
			List<Space> closerSpaces = new List<Space>();
			if(bot.x!=destination.x)
				closerSpaces.Add(new Space(_CloserCoord(bot.x, destination.x), bot.y));
			if(bot.y!=destination.y)
				closerSpaces.Add(new Space(bot.x, _CloserCoord(bot.y, destination.y)));

			return closerSpaces.Count > 0 ? _PickARandomSpace(closerSpaces) : bot;
		}

		private static int _CloserCoord(int botCoord, int destinationCoord)
		{
			return botCoord + (int)((destinationCoord - botCoord )/ Math.Abs(destinationCoord - botCoord));
		}

	    public string Name { get { return "GramBot"; } }
	    public string Color { get { return "Red"; }}
	    
	}
}