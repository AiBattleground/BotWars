var gridUtils = require("./gridUtils");

var brain = function () {

    function think(perception) {

        try {

            var thoughts = {
                individualDestinationsDirect: [],
                individualDestinationsAstar: [],
                groupDestinationsDirect: [],
                groupDestinationsAstar: []
            };

            // Pull Data
            var myUnitCoords = cloneCoordList(perception.myUnitCoords);

            // Clear Spawn of Food First
            //if (doesSpawnHaveEnergy(perception)) {
            //    var closestUnitToMySpawn = getClosestUnitToSpawn(perception.mySpawnCoord, myUnitCoords);
            //    thoughts.individualDestinationsDirect.push(getDestination(closestUnitToMySpawn, perception.mySpawnCoord));
            //}

            // Seek Energy
            var countOfUnitsSeekeingEnergy = getCountOfUnitsSeekingEnergy(perception, myUnitCoords, perception.energyCoords);
            var energySeekerDestinations = getClosestUnitDestinations(
                                                                perception.gridSize,
                                                                perception.energyCoords,
                                                                myUnitCoords,
                                                                countOfUnitsSeekeingEnergy);

            thoughts.individualDestinationsAstar = combineArrays(
                                                        thoughts.individualDestinationsAstar,
                                                        energySeekerDestinations);

            //logDestinationList(individualEnergySeekerDestinations);
            //console.log(myUnitCoords.length);

            var unitsLeftToConsider = getUnitsRemaining(myUnitCoords, energySeekerDestinations);

            // Allocate remaining Units
            var unitsLeftToConsiderCount = unitsLeftToConsider.length;
            var enemySpawnAttackersCount;
            var enemyUnitAttackerCount;
            var defenseUnitCount;

            if (perception.enemySpawnRazed && perception.mySpawnRazed) {
                enemySpawnAttackersCount = 0;
                defenseUnitCount = 0;
                enemyUnitAttackerCount = unitsLeftToConsiderCount;
            }
            else if (perception.enemySpawnRazed) {
                enemySpawnAttackersCount = 0;
                //defenseUnitCount = Math.floor(unitsLeftToConsiderCount / 2);
                defenseUnitCount = 0;
                enemyUnitAttackerCount = unitsLeftToConsiderCount - defenseUnitCount;
            }
            else if (perception.mySpawnRazed) {
                enemySpawnAttackersCount = Math.floor(unitsLeftToConsiderCount / 2);
                defenseUnitCount = 0;
                enemyUnitAttackerCount = unitsLeftToConsiderCount - enemySpawnAttackersCount;
            }
            else {
                enemySpawnAttackersCount = Math.floor(unitsLeftToConsiderCount * 4 / 5);
                //enemyUnitAttackerCount = Math.floor(unitsLeftToConsiderCount * 3 / 5);
                enemyUnitAttackerCount = unitsLeftToConsiderCount - enemySpawnAttackersCount;
                //defenseUnitCount = unitsLeftToConsiderCount - enemySpawnAttackersCount - enemyUnitAttackerCount;
                defenseUnitCount = 0;
            }

            console.log("Unit Count: " + perception.myUnitCoords.length);
            console.log("Unit Remaining Count: " + unitsLeftToConsiderCount);
            console.log("Unit Spawn Attack Count: " + enemySpawnAttackersCount);
            console.log("Unit Enemy Attack Count: " + enemyUnitAttackerCount);
            console.log("Unit Defense Count: " + defenseUnitCount);


            //console.log(myUnitCoords.length);

            // Rush Enemy Base
            if (enemySpawnAttackersCount > 0) {
                var enemySpawnAttackers = unitsLeftToConsider.splice(0, enemySpawnAttackersCount);

                var enemySpawnAttackDestinations = directUnitsToTarget(perception.enemySpawnCoord, enemySpawnAttackers);

                thoughts.individualDestinationsDirect = combineArrays(
                                                            thoughts.individualDestinationsDirect,
                                                            enemySpawnAttackDestinations);
            }

            // Attack Enemy Units
            if (unitsLeftToConsider.length > 0) {
                var enemyAttackerDestinations = [];

                unitsLeftToConsider.forEach(
                    function (attackingUnit) {
                        //console.log(enemyCoord);
                        var closestEnemyCoord = closestUnitToCoord(perception.gridSize, perception.enemyUnitCoords, attackingUnit);
                        //console.log(closestUnitCoord);
                        //console.log("{ x: " + closestUnitCoord.x + ", y: " + closestUnitCoord.y + " }");
                        var attackDestination = getDestination(attackingUnit, closestEnemyCoord);
                        enemyAttackerDestinations.push(attackDestination);
                    }
                );

                thoughts.individualDestinationsDirect = combineArrays(
                                                            thoughts.individualDestinationsDirect,
                                                            enemyAttackerDestinations);
            }
            
            //logDestinationList(spawnKillerDestinations);

            return thoughts;
        }
        
        catch (ex) {
            console.log(ex.message);
            console.log(ex.stack);
        }
    }

    function popUnitClosestToEnemySpawn(perception, unitCoords) {
        var indexOfCoordClosestToEnemySpawn = 0;
        var minDistance = perception.gridSize.cols + perception.gridSize.rows + 1;
        var enemySpawnCoord = perception.enemySpawnCoord;

        for (var i = 0; i < unitCoords.length; i++) {
            var distance = gridUtils.getManhattanDistance(unitCoords[i], enemySpawnCoord);
            if (distance < minDistance) {
                minDistance = distance;
                indexOfCoordClosestToEnemySpawn = i;
            }
        }

        var closestCoord = unitCoords[indexOfCoordClosestToEnemySpawn];

        unitCoords.splice(indexOfCoordClosestToEnemySpawn, 1);
        
        return closestCoord;
    }

    function closestUnitToEnemySpawn(gridSize, unitCoords, enemySpawnCoord) {
        return closestUnitToCoord(gridSize, unitCoords, enemySpawnCoord);
    }

    function closestUnitToCoord(gridSize, unitCoords, target) {
        var minDistance = gridSize.cols + gridSize.rows + 1;
        var closestUnitCoord = unitCoords[0];

        unitCoords.forEach(
            function (unitCoord) {
                var distance = gridUtils.getManhattanDistance(unitCoord, target);

                if (distance < minDistance) {
                    minDistance = distance;
                    closestUnitCoord = unitCoord;
                }
            }
        );

        return closestUnitCoord;
    }

    function closestCoordToAdjacentSpaceOfTarget(gridSize, coords, target) {
        var targetNeighbors = gridUtils.getNeighborCoordsUpToDistance(gridSize, target, 1);

        var closestCoords = [];

        targetNeighbors.forEach(
            function (neighbor) {
                closestCoords.push(closestUnitToCoord(gridSize, coords, neighbor));
            }
        );

        return closestCoords[0];

        var minDistance = 1000;
        var closestCoord = null;

        closestCoords.forEach(
            function (coord) {
                var distance = gridUtils.getManhattanDistance(coord, target);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestCoord = coord;
                }
            }
        );

        return closestCoord;
    }

    function getCountOfUnitsSeekingEnergy(perception, unitCoords, energyCoords) {

        if (isEnergyCollectionNeeded(perception)) {
            return getShorterLengthOfTwoArrays(unitCoords, energyCoords);
        }

        return 0;
    }

    function getShorterLengthOfTwoArrays(array1, array2) {
        var array1Count = array1.length;
        var array2Count = array2.length;

        if (array1Count > array2Count) {
            return array2Count;
        }
        else {
            return array1Count;
        }
    }

    function getClosestUnitDestinations(gridSize, targetCoords, unitCoords, unitCount) {
        var unitDestinations = [];
        var potentialDestinations = getPotentialDestinations(gridSize, targetCoords, unitCoords);

        var numberOfUnitsProcessed = 0;

        while (numberOfUnitsProcessed < unitCount) {

            var closestDestination = findClosestDestination(gridSize, potentialDestinations);

            unitDestinations.push(getDestination(
                                        closestDestination.unitCoord,
                                        closestDestination.closestDestNeighborCoord));

            removeAllDestinationsWithUnit(potentialDestinations, closestDestination.unitCoord);
            removeAllDestinationsWithTarget(potentialDestinations, closestDestination.targetCoord);

            numberOfUnitsProcessed++;
        }

        return unitDestinations;
    }

    function isEnergyCollectionNeeded(perception) {

        return perception.myUnitCoords && perception.energyCoords &&
            perception.myUnitCoords.length > 0 && perception.energyCoords.length > 0;
    }

    function findClosestDestination(gridSize, destinations) {
        var shortestDistance = gridSize.cols + gridSize.rows + 1;
        var closestDestination = null;

        destinations.forEach(
            function (destination) {
                if (destination.distance < shortestDistance) {
                    shortestDistance = destination.distance;
                    closestDestination = destination;
                }
            }
        );

        return closestDestination;
    }

    function removeAllDestinationsWithUnit(pairs, unitCoord) {
        var indicesToRemove = [];

        for (var i = 0; i < pairs.length; i++) {
            if (pairs[i].unitCoord === unitCoord) {
                indicesToRemove.push(i);
            }
        }

        return removeItemsAtIndicesFromArray(pairs, indicesToRemove);
    }

    function removeAllDestinationsWithTarget(pairs, targetCoord) {
        var indicesToRemove = [];

        for (var i = 0; i < pairs.length; i++) {
            if (pairs[i].targetCoord === targetCoord) {
                indicesToRemove.push(i);
            }
        }

        return removeItemsAtIndicesFromArray(pairs, indicesToRemove);
    }

    function removeItemsAtIndicesFromArray(array, indicesToRemove) {
        for (var i = 0; i < indicesToRemove.length; i++) {
            array.splice(indicesToRemove[i] - i, 1);
        }

        return array;
    }

    function getPotentialDestinations(gridSize, targetCoords, unitCoords) {
        var unitDestinations = [];

        unitCoords.forEach(
            function (unitCoord) {
                targetCoords.forEach(
                    function (targetCoord) {
                        var destination = gridUtils.getClosestDestinationNeighborAndDistance(gridSize, unitCoord, targetCoord);
                        unitDestinations.push({
                            unitCoord: unitCoord,
                            targetCoord: targetCoord,
                            closestDestNeighborCoord: destination.closestDestinationNeighborCoord,
                            distance: destination.distance
                        });
                    }
                );
            }
        );

        return unitDestinations;
    }

    function getUnitsRemaining(myUnitCoords, individualEnergySeekerDestinations) {

        individualEnergySeekerDestinations.forEach(
            function (destination) {
                myUnitCoords.forEach(
                    function (unitCoord) {
                        if (unitCoord === destination.unitCoord) {
                            var indexOfUnitCoord = myUnitCoords.indexOf(unitCoord);
                            myUnitCoords.splice(indexOfUnitCoord, 1);
                        }
                    }
                );
            }
        );

        return myUnitCoords;
    }

    function directUnitsToTarget(targetCoord, unitCoords) {
        var unitDestinations = [];

        unitCoords.forEach(
            function (unitCoord) {
                unitDestinations.push(getDestination(unitCoord, targetCoord));
            }
        );

        return unitDestinations;
    }

    function getDestination(sourceCoord, destinationCoord) {
        return {
            unitCoord: sourceCoord,
            targetCoord: destinationCoord
        };
    }

    function cloneCoordList(listToCopy) {

        var newList = [];

        listToCopy.forEach(
            function (coord) {
                newList.push({ x: coord.x, y: coord.y });
            }
        );

        return newList;
    }

    function combineArrays(array1, array2) {
        // ARRAY.CONCAT
        array2.forEach(
            function (item) {
                array1.push(item);
            }
        );

        return array1;
    }

    function logDestinationList(destinationList) {
        destinationList.forEach(
            function (destination) {
                console.log("Destination");
                console.log(gridUtils.coordToText(destination.unitCoord));
                console.log(gridUtils.coordToText(destination.targetCoord));
                console.log("\n");
            }
        );
    }

    return {
        name: "brain",
        think: think
    }
}();

module.exports = brain;