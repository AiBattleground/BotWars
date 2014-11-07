var gridToTextUtils = require("./gridToTextUtils");
var gridUtils = require("./gridUtils");
var arrayCoordUtils = require("./arrayCoordUtils");
var aStar = require("./astar.js");

var actuator = function () {

    function act(perception, thoughts) {

        var actions = [];

        // Build standard 'all weights' path grid
        var pathCosts = buildCollisionAvoidancePathValues(perception.gridSize);
        var pathGridWithAllWeights = buildInitialPathGrid(perception);
        addAllCostsToPathGrid(perception, pathGridWithAllWeights, pathCosts);

        // Build path grid with only friendly unit costs to ensure no collisions between my units
        var pathGridWithMyUnitWeightsOnly = buildInitialPathGrid(perception);
        addCostsOfFriendlyUnitsToPathGrid(perception.gridSize, perception.myUnitCoords,
            pathGridWithMyUnitWeightsOnly, pathCosts);

        gridToTextUtils.logPathGrid(perception.gridSize, pathGridWithMyUnitWeightsOnly);
        gridToTextUtils.logPathGrid(perception.gridSize, pathGridWithAllWeights);

        thoughts.individualDestinationsAstar.forEach(
            function (unitDestination) {
                var delta = aStar.getDelta(
                                    perception.gridSize,
                                    pathGridWithAllWeights,
                                    unitDestination.unitCoord,
                                    unitDestination.targetCoord
                                );

                //console.log("delta: " + delta);

                if (delta !== null) {
                    updateGrid(pathGridWithAllWeights, pathCosts, unitDestination.unitCoord, delta);
                    updateGrid(pathGridWithMyUnitWeightsOnly, pathCosts, unitDestination.unitCoord, delta);
                    actions.push(gridToTextUtils.getAction(perception.gridSize, unitDestination.unitCoord, delta));
                }
            }
        );

        thoughts.individualDestinationsDirect.forEach(
            function (unitDestination) {
                var delta = aStar.getDelta(
                                    perception.gridSize,
                                    pathGridWithMyUnitWeightsOnly,
                                    unitDestination.unitCoord,
                                    unitDestination.targetCoord
                                );

                //console.log("delta: " + delta);

                if (delta !== null) {
                    updateGrid(pathGridWithAllWeights, pathCosts, unitDestination.unitCoord, delta);
                    updateGrid(pathGridWithMyUnitWeightsOnly, pathCosts, unitDestination.unitCoord, delta);
                    actions.push(gridToTextUtils.getAction(perception.gridSize, unitDestination.unitCoord, delta));
                }
            }
        );

        return actions;
    }

    function updateGrid(grid, pathCosts, coord, delta) {
        grid[coord.x][coord.y] -= pathCosts.friendlyUnitPathCost;
        grid[coord.x + delta.dx][coord.y + delta.dy] += pathCosts.friendlyUnitPathCost;
    }

    function generateMoveTowardsGoal(gridSize, unitCoord, targetCoord) {

        var delta = generateDirectPathDeltaWithNoCollisionAvoidance(unitCoord, targetCoord);

        return gridToTextUtils.getAction(gridSize, unitCoord, delta);
    }

    function buildCollisionAvoidancePathValues(gridSize) {
        return {
            enemyUnitPathCost: 5,
            friendlyUnitPathCost: 999,
            energyPathCost: -1
        };
    }

    function buildInitialPathGrid(perception) {

        var pathGrid = [];

        for (var x = 0; x < perception.gridSize.cols; x++) {
            var newColumn = [];
            for (var y = 0; y < perception.gridSize.rows; y++) {
                newColumn.push(0);
            }
            pathGrid.push(newColumn);
        }

        return pathGrid;
    }

    function addAllCostsToPathGrid(perception, pathGrid, pathCosts) {
        addCostsOfEnemyUnitsToPathGrid(perception.gridSize, perception.enemyUnitCoords, pathGrid, pathCosts);
        addCostsOfEnergyToPathGrid(perception.gridSize, perception.energyCoords, pathGrid, pathCosts);
        addCostsOfFriendlyUnitsToPathGrid(perception.gridSize, perception.myUnitCoords, pathGrid, pathCosts);
    }

    function addCostsOfEnemyUnitsToPathGrid(gridSize, enemyUnitsCoords, pathGrid, pathCosts) {
        enemyUnitsCoords.forEach(
            function (enemyUnitCoord) {
                pathGrid[enemyUnitCoord.x][enemyUnitCoord.y] += pathCosts.enemyUnitPathCost;
                var enemyUnitNeighborCoords = gridUtils.getNeighborCoordsUpToDistance(gridSize, enemyUnitCoord, 2);

                enemyUnitNeighborCoords.forEach(
                    function (neighborCoord) {
                        if (!(arrayCoordUtils.coordEquals(neighborCoord, enemyUnitCoord))) {
                            var manhattanDistance = gridUtils.getManhattanDistance(enemyUnitCoord, neighborCoord);
                            pathGrid[neighborCoord.x][neighborCoord.y] += Math.floor(pathCosts.enemyUnitPathCost / (manhattanDistance + 1));
                        }
                    }
                );
            }
        );
    }

    function addCostsOfEnergyToPathGrid(gridSize, energyCoords, pathGrid, pathCosts) {
        energyCoords.forEach(
            function (energyCoord) {
                pathGrid[energyCoord.x][energyCoord.y] += pathCosts.energyPathCost;
            }
        );
    }

    function addCostsOfFriendlyUnitsToPathGrid(gridSize, friendlyUnitCoords, pathGrid, pathCosts) {
        friendlyUnitCoords.forEach(
            function (friendlyCoord) {
                pathGrid[friendlyCoord.x][friendlyCoord.y] += pathCosts.friendlyUnitPathCost;
            }
        );
    }

    function generateDirectPathDeltaWithNoCollisionAvoidance(agentCoord, goalCoord) {
        var xDistance = agentCoord.x - goalCoord.x;
        var yDistance = agentCoord.y - goalCoord.y;

        var xAbsDistance = Math.abs(xDistance);
        var yAbsDistance = Math.abs(yDistance);

        var delta = { dx: 0, dy: 0 };

        if (xAbsDistance > yAbsDistance) {
            if (xAbsDistance === 0) {
                delta.dx = 0;
            }
            else {
                delta.dx = xDistance / xAbsDistance * -1;
            }
        }
        else {
            if (yAbsDistance === 0) {
                delta.dy = 0;
            }
            else {
                delta.dy = yDistance / yAbsDistance * -1;
            }
        }

        return delta;
    }

    function generateSingleZeroDistanceMove(gridSize, unitCoord) {
        var zeroDelta = { dx: 0, dy: 0 };

        return generateMoveTowardsGoal(gridSize, unitCoord, unitCoord);
    }

    return {
        name: "actuator",
        act: act
    }
}();

module.exports = actuator;