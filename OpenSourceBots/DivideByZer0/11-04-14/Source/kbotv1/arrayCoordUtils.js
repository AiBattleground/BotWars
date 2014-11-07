

var arrayCoordUtils = function () {

    function mergeCoordArrays(array1, array2) {

        var indicesToAddToArray2 = [];
        array1.forEach(
            function (item) {
                var index = indexOfCoord(array2, item);
                if (index === -1) {
                    indicesToAddToArray2.push(array1.indexOf(item));
                }
            }
        );

        indicesToAddToArray2.forEach(
            function (index) {
                array2.push(array1[index]);
            }
        );

        return array2;
    }

    function removeDuplicateCoordsInPlace (arr) {
        var i, j, cur, found;
        for (i = arr.length - 1; i >= 0; i--) {
            cur = arr[i];
            found = false;
            for (j = i - 1; !found && j >= 0; j--) {
                if (coordEquals(cur,arr[j])) {
                    if (i !== j) {
                        arr.splice(i, 1);
                    }
                    found = true;
                }
            }
        }

        return arr;
    };

    function indexOfCoord(arrayOfCoords, coordToFind) {
        for (var i = 0; i < arrayCoordUtils.length; i++) {
            if (coordEquals(coordToFind, arrayOfCoords[i])) {
                return i;
            }
        }

        return -1;
    }

    function coordEquals(coord1, coord2) {
        return coord1.x === coord2.x && coord1.y === coord2.y;
    }

    return {
        mergeCoordArrays: mergeCoordArrays,
        removeDuplicateCoordsInPlace: removeDuplicateCoordsInPlace,
        coordEquals: coordEquals,
        indexOfCoord: indexOfCoord
    }

}();

module.exports = arrayCoordUtils;