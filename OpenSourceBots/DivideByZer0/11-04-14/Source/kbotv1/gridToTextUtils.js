

var gridToTextUtils = function () {

    function getAction(gridSize, coord, delta) {

        //console.log("getAction - gridSize: " + gridSize);
        //console.log("getAction - coord: " + coord);
        //console.log("getAction - delta: " + delta);

        var startingIndex = coordToIndex(gridSize, coord);
        var destinationCoord = { x: coord.x + delta.dx, y: coord.y + delta.dy };
        var destinationIndex = coordToIndex(gridSize, destinationCoord);

        return { from: startingIndex, to: destinationIndex };
    }

    function getAllIndices(grid, search) {
        var arr = [];
        if (search === '.') search = '\\.';
        if (search === '*') search = '\\*';
        var re = new RegExp(search, 'g');
        while (m = re.exec(grid)) {
            arr.push(m.index);
        }

        return arr;
    }

    function coordToIndex(gridSize, coord) {
        return coord.x + coord.y * gridSize.cols;
    }

    function logArena(gameState) {
        var sGrid = gameState.grid;
        var iNumberOfColumns = gameState.cols;

        var sBorderUpperLeft = "+";
        var sBorderUpperRight = "+";
        var sBorderLowerLeft = "+";
        var sBorderLowerRight = "+";

        var sBorderHorizontal = getStringOfChars("-", iNumberOfColumns * 3 + 2);
        var sBorderVertical = "|";

        var sArena = "\n" + sBorderUpperLeft + sBorderHorizontal + sBorderUpperRight + "\n";

        for (var i = 0; sGrid.length > i; i = i + iNumberOfColumns) {
            sArena += sBorderVertical + " ";
            for (var c = 0; c < iNumberOfColumns; c++) {
                sArena += " " + sGrid.substr(i + c, 1) + " ";
            }
            sArena += " " + sBorderVertical + "\n";
        }

        sArena += sBorderLowerLeft + sBorderHorizontal + sBorderLowerRight + "\n";

        console.log(sArena);
    }

    function logPathGrid(gridSize, pathGrid) {
        var sBorderUpperLeft = "+";
        var sBorderUpperRight = "+";
        var sBorderLowerLeft = "+";
        var sBorderLowerRight = "+";

        var sBorderHorizontal = getStringOfChars("-", gridSize.cols * 3 + 2);
        var sBorderVertical = "|";

        var sArena = "\n" + sBorderUpperLeft + sBorderHorizontal + sBorderUpperRight + "\n";

        for (var y = 0; y < gridSize.rows; y++) {
            sArena += sBorderVertical + " ";
            for (var x = 0; x < gridSize.cols; x++) {
                var value = Number(pathGrid[x][y]);
                var lengthOfValue = value.length;

                if (valueIsNotThreeDigits(value)) {
                    sArena += " ";
                }

                sArena += value;

                if (valueIsNotTwoDigits(value)) {
                    sArena += " ";
                }
            }
            sArena += " " + sBorderVertical + "\n";
        }

        sArena += sBorderLowerLeft + sBorderHorizontal + sBorderLowerRight + "\n";

        console.log(sArena);
    }

    function valueIsNotThreeDigits(value) {
        if (value > 0 && value < 100) {
            return true;
        }

        if (value <= 0 && Math.abs(value) < 10) {
            return true;
        }

        return false;
    }

    function valueIsNotTwoDigits(value) {
        if (value >= 0 && value < 10) {
            return true;
        }

        return false;
    }

    function getStringOfChars(sChar, iLength) {
        var sStringOfChars = "";
        while (iLength !== 0) {
            sStringOfChars += sChar;
            iLength--;
        }
        return sStringOfChars;
    }

    return {
        getAction: getAction,
        getAllIndices: getAllIndices,
        logArena: logArena,
        logPathGrid: logPathGrid
    }

}();

module.exports = gridToTextUtils;