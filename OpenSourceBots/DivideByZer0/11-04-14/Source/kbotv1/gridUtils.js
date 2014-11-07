var gridToTextUtils = require("./gridToTextUtils");
var arrayCoordUtils = require("./arrayCoordUtils");

var gridUtils = function () {

    function getAllCoords(perception, search) {
        var indices = gridToTextUtils.getAllIndices(perception.grid, search);
        var coords = [];
        for (var i = 0; indices.length > i; i++) {
            var index = indices[i];
            coords.push(indexToCoord(perception.gridSize, index));
        }

        return coords;
    }

    function indexToCoord(gridSize, index) {
        var x = Number(index) % Number(gridSize.cols);
        var y = ~~(Number(index) / Number(gridSize.rows));
        return { x: x, y: y };
    }

    function getClosestDestinationNeighborAndDistance(gridSize, sourceCoord, destCoord) {
        var minDistance = gridSize.cols + gridSize.rows;
        var closestNeighborCoord = destCoord;

        var neighborCoords = getNeighborCoords(gridSize, destCoord);

        neighborCoords.forEach(
            function (neighborCoord) {
                var distance = getManhattanDistance(sourceCoord, neighborCoord);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestNeighborCoord = neighborCoord;
                }
            }
        );

        return { closestDestinationNeighborCoord: closestNeighborCoord, distance: minDistance };
    }

    function getNeighborCoords(gridSize, coord) {
        return getNeighborCoordsUpToDistance(gridSize, coord, 1);
    }

    function getNeighborCoordsUpToDistance(gridSize, coord, distance) {
        var immediateNeighbors = getImmediateNeighborCoords(gridSize, coord);

        if (distance === 1) {
            return immediateNeighbors;
        }

        var totalNeighbors = [];

        immediateNeighbors.forEach(
            function (neighbor) {
                var nextLevelNeighbors = getNeighborCoordsUpToDistance(gridSize, neighbor, distance - 1);
                totalNeighbors = arrayCoordUtils.mergeCoordArrays(nextLevelNeighbors, totalNeighbors);
            }
        );

        totalNeighbors = arrayCoordUtils.mergeCoordArrays(immediateNeighbors, totalNeighbors);

        var indexOfCoord = arrayCoordUtils.indexOfCoord(totalNeighbors, coord);

        while (indexOfCoord !== -1) {
            //console.log("index: " + indexOfCoord);
            totalNeighbors.splice(indexOfCoord, 1);
            indexOfCoord = arrayCoordUtils.indexOfCoord(totalNeighbors, coord);
        }

        totalNeighbors = arrayCoordUtils.removeDuplicateCoordsInPlace(totalNeighbors);

        return totalNeighbors;
    }

    function getNeighborCoordsAtDistance(gridSize, coord, distance) {
        var allNeighborsWithinDistance = getNeighborCoordsUpToDistance(gridSize, coord, distance);

        if (distance === 1) {
            return allNeighborsWithinDistance;
        }

        var neighborsAtDistance = [];

        allNeighborsWithinDistance.forEach(
            function (neighbor) {
                if (getManhattanDistance(coord, neighbor) === distance) {
                    neighborsAtDistance.push(neighbor);
                }
            }
        );

        return neighborsAtDistance;
    }

    function getImmediateNeighborCoords(gridSize, coord) {
        var neighborCoords = [];

        if (coord.x > 0) {
            neighborCoords.push({ x: coord.x - 1, y: coord.y });
        }

        if (coord.y > 0) {
            neighborCoords.push({ x: coord.x, y: coord.y - 1 });
        }

        if (coord.x < gridSize.cols - 1) {
            neighborCoords.push({ x: coord.x + 1, y: coord.y });
        }

        if (coord.y < gridSize.rows - 1) {
            neighborCoords.push({ x: coord.x, y: coord.y + 1 });
        }

        return neighborCoords;
    }

    function getManhattanDistance(coord1, coord2) {
        return Math.abs(coord1.x - coord2.x) + Math.abs(coord1.y - coord2.y);
    }

    function coordToText(coord) {
        return "{ x: " + coord.x + ", y: " + coord.y + " }";
    }

    return {
        getAllCoords: getAllCoords,
        indexToCoord: indexToCoord,
        getManhattanDistance: getManhattanDistance,
        getClosestDestinationNeighborAndDistance: getClosestDestinationNeighborAndDistance,
        getNeighborCoordsUpToDistance: getNeighborCoordsUpToDistance,
        coordToText: coordToText
    }
    
}();

module.exports = gridUtils;