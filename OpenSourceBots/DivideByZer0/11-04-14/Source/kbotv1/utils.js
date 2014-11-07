

var utils = function () {

    /**
    
     * @description describe( vUnknown ) Alerts all of the attribute names and values present in the the object/variant passed in.           Useful when an obect is behaving as those it's not of the right type, or you can't figure out what object you have to work with.
    
     * @version 1.1
    
     * @param {Variant} vUnknown Any "thing" that can be passed by reference in javascript.           May be a string, number, object, array, function, etc. - hence the type variant.
    
     * @return String             the tab delimited list of properties with the header info is returned (after being alerted);
    
     * @author Quincy L. Acklen
    
     * @copyright Copyright (c) 1999-2006 The UI Side. All rights reserved.
    
     */

    function describe(vUnknown) {

        var sAttributes = "Description of -->" + vUnknown + "<-- ( " + typeof vUnknown + " )\n\n\n";

        for (var x in vUnknown) {
            if (vUnknown[x] != null) {
                if (vUnknown.hasOwnProperty(vUnknown[x])) {
                    sAttributes += describe(vUnknown[x]);
                }
                else {
                    sAttributes += x + ": \t" + vUnknown[x] + "\n";
                }
            }
        }

        console.log(sAttributes);

        return sAttributes;

    }

    return {
        describe: describe
    };
}();

module.exports = utils;