var gridUtils = require("./gridUtils");
var gridToTextUtils = require("./gridToTextUtils");
var utils = require("./utils");

var sensor = function () {

    function perceive(state, player) {

        try {
            /*
              "state": {
                  "rows": 4,
                  "cols": 4,
                  "p1": {
                    "energy": 0,
                    "spawn": 5
                  },
                  "p2": {
                    "energy": 0,
                    "spawn": 10
                  },
                  "grid": ".....r....b.....",
                  "maxTurns": 20,
                  "turnsElapsed": 0
                  },
                  "player": "r"
            */

            //utils.describe(state);

            console.log("Turn #" + (Number(state.turnsElapsed) + 1));

            gridToTextUtils.logArena(state);

            var perception = {};

            perception.gridSize = { cols: state.cols, rows: state.rows };
            perception.grid = state.grid;

            perception.turnsRemaining = Number(state.maxTurns) - Number(state.turnsElapsed) - 1;

            perception.energyCoords = gridUtils.getAllCoords(perception, '*');

            if (player === 'p1') {
                perception.myEnergy = state.p1.energy;
                perception.mySpawnCoord = gridUtils.indexToCoord(perception.gridSize, state.p1.spawn);
                perception.mySpawnRazed = state.p1.spawnDisabled;
                perception.enemyEnergy = state.p2.energy;
                perception.enemySpawnCoord = gridUtils.indexToCoord(perception.gridSize, state.p2.spawn);
                perception.enemySpawnRazed = state.p2.spawnDisabled;
                perception.myUnitCoords = gridUtils.getAllCoords(perception, '1');
                perception.enemyUnitCoords = gridUtils.getAllCoords(perception, '2');
            }
            else {
                perception.myEnergy = state.p2.energy;
                perception.mySpawnCoord = gridUtils.indexToCoord(perception.gridSize, state.p2.spawn);
                perception.mySpawnRazed = state.p2.spawnDisabled;
                perception.enemyEnergy = state.p1.energy;
                perception.enemySpawnCoord = gridUtils.indexToCoord(perception.gridSize, state.p1.spawn);
                perception.enemySpawnRazed = state.p1.spawnDisabled;
                perception.myUnitCoords = gridUtils.getAllCoords(perception, '2');
                perception.enemyUnitCoords = gridUtils.getAllCoords(perception, '1');
            }

            //logPerception(perception);

            return perception;
        }

        catch (ex) {
            console.log(ex);
        }
    }

    function logPerception(perception) {
        console.log("         PERCEPTION");
        console.log("----------------------------");
        console.log("GridSize: { x: " + perception.gridSize.cols + ", y: " + perception.gridSize.rows + " }");
        console.log("Turns Remaining: " + perception.turnsRemaining);
        console.log("Enemy Spawn Coord: " + gridUtils.coordToText(perception.enemySpawnCoord));
    }

    return {
        name: "sensor",
        perceive: perceive
    }
}();

module.exports = sensor;