var sensor = require("./standardPerceive");
var brain = require("./energyGatheringThink");
var actuator = require("./energyGatheringActuator");

var agent = function () {

    function applyAI(state, player) {

        var perception = sensor.perceive(state, player);

        var thoughts = brain.think(perception);

        var actions = actuator.act(perception, thoughts);

        return actions;
    }

    return {
        name : "agent",
        applyAI: applyAI
    }
}();

module.exports = agent;
