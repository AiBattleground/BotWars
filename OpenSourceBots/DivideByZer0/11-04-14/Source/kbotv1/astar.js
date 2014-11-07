var gridUtils = require("./gridUtils");
var arrayCoordUtils = require("./arrayCoordUtils");

var aStar = function () {

    function getDelta(gridSize, gridPathCosts, sourceCoord, goalCoord) {
        var openList = [];
        var closedList = [];

        openList.push(makeNode(sourceCoord, goalCoord, 0, null));

        var searchInProgress = true;
        var pathFound = false;
        var bestNode = null;
        var neighborCoords = null;
        var childCandidates = null;

        while (searchInProgress) {

            //console.log("--------------- start aStar loop -----------------");
            //logNodeList("openList", openList);
            //console.log("\n");
            //logNodeList("closedList", closedList);
            //console.log("\n");

            if (openList.length === 0) {
                //console.log("NO PATH FOUND");
                searchInProgress = false;
                pathFound = false;
            }
            else {
                bestNode = popBestNodeFromList(openList);
                //console.log("Best Node");
                //logNode(bestNode);
                //console.log("\n");

                if (arrayCoordUtils.coordEquals(bestNode.coord, goalCoord)) {
                    //console.log("PATH FOUND");
                    searchInProgress = false;
                    pathFound = true;
                }
                else {
                    closedList.push(bestNode);

                    childCandidates = createChildrenNodes(bestNode, gridSize, goalCoord, gridPathCosts);

                    //logNodeList("Child Candidates", childCandidates);

                    childCandidates.forEach(
                        function (childNodeCandidate) {
                            var indexOnOpenList = findNode(childNodeCandidate, openList);
                            var indexOnClosedList = findNode(childNodeCandidate, closedList);

                            if (indexOnOpenList !== -1) {

                                //console.log("found on OPEN list");

                                bestNode.children.push(childNodeCandidate);
                                if (childNodeCandidate.g < openList[indexOnOpenList].g) {
                                    childNodeCandidate.parent = bestNode;
                                }
                            }
                            else if (indexOnClosedList !== -1) {

                                //console.log("found on CLOSED list");

                                bestNode.children.push(childNodeCandidate);
                                if (childNodeCandidate.g < closedList[indexOnClosedList].g) {
                                    childNodeCandidate.parent = bestNode;

                                    updateParents(childNodeCandidate, gridPathCosts);
                                }
                            }
                            else {

                                //console.log("added to OPEN list");

                                openList.push(childNodeCandidate);
                            }
                        }
                    );
                }
            }
        }

        if (pathFound) {
            var nextNode = bestNode;

            while (bestNode.parent !== null) {
                nextNode = bestNode;
                bestNode = bestNode.parent;
            }

            return { dx: nextNode.coord.x - sourceCoord.x, dy: nextNode.coord.y - sourceCoord.y };
        }

        return null;
    }

    function updateParents(node, gridPathCosts) {
        var stack = [];
        var childrenCount = node.children.length;
        var child = null;

        for (var i = 0; i < childrenCount; i++) {
            child = node.children[i];

            if ((node.g + 1) < child.g) {
                child.g = g + 1;
                child.f = child.g + child.h;
                child.parent = node;

                stack.push(child);
            }
        }

        var parent = null;

        while (stack.length > 0) {
            parent = stack.pop();

            childrenCount = parent.children.length;

            for (var i = 0; i < childrenCount; i++) {
                child = parent.children[i];

                if ((parent.g + 1) < child.g) {
                    child.g = parent.g + gridPathCosts[child.coord.x][child.coord.y];
                    child.f = child.g + child.h;
                    child.parent = parent;

                    stack.push(child);
                }
            }
        }
    }

    function makeNode(coord, goalCoord, cost, parentNode) {
        // g = cost to get from the starting node to this node
        // h = estimated cost to get from this node to the goal (manhattan distance)
        // f = g + h

        var g;

        if (parentNode === null) {
            g = 0;
        }
        else {
            g = parentNode.g + cost;
        }

        var h = gridUtils.getManhattanDistance(coord, goalCoord);

        return {
            coord: coord,
            g: g,
            h: h,
            f: g + h,
            parent: parentNode,
            children: []
        };
    }

    function createChildrenNodes(parentNode, gridSize, goalCoord, gridPathCosts) {
        neighborCoords = gridUtils.getNeighborCoordsUpToDistance(gridSize, parentNode.coord, 1);

        var childrenNodes = [];

        neighborCoords.forEach(
            function (neighborCoord) {
                //console.log("neighborCoord: " + neighborCoord);
                //console.log("goalCoord: " + goalCoord);
                //console.log("gridPathValue: " + gridPathCosts[neighborCoord.x][neighborCoord.y]);
                //console.log("parentNode: " + parentNode);
                childrenNodes.push(makeNode(neighborCoord, goalCoord, gridPathCosts[neighborCoord.x][neighborCoord.y], parentNode));
            }
        );

        return childrenNodes;
    }

    function findNode(node, list) {
        for (var i = 0; i < list.length; i++) {
            if (arrayCoordUtils.coordEquals(node.coord, list[i].coord)) {
                return i;
            }
        }

        return -1;
    }

    function logNodeList(listName, nodeList) {
        console.log(listName);
        console.log("----------------------");

        if (nodeList.length === 0) {
            console.log("Empty");
        }
        else {
            nodeList.forEach(
                function (node) {
                    logNode(node);
                }
            );
        }
    }

    function logNode(node) {
        console.log("coord: " + gridUtils.coordToText(node.coord));
        if (node.parent === null) {
            console.log("parent: null");
        }
        else {
            console.log("parent: " + gridUtils.coordToText(node.parent.coord));
        }
        console.log("g: " + node.g);
        console.log("h: " + node.h);
        console.log("f: " + node.f);
        if (node.children.length === 0) {
            console.log("children: none");
        }
        else {
            console.log("children: ");
            node.children.forEach(
                function (childNode) {
                    console.log("  " + gridUtils.coordToText(childNode.coord));
                }
            );
        }
    }

    function popBestNodeFromList(list) {
        var lowestF = 1000;
        var bestNodeIndex = 0;

        for (var i = 0; i < list.length; i++) {
            if (list[i].f < lowestF) {
                lowestF = list[i].f;
                bestNodeIndex = i;
            }
        }

        var bestNode = list[bestNodeIndex];

        list.splice(bestNodeIndex, 1);
        
        return bestNode;
    }

    return {
        getDelta: getDelta
    }

}();

module.exports = aStar;