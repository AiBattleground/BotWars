// Original example bot from Skookum

var fs = require('fs');
var util = require('util');
var http = require('http');
var express = require('express');
var agent = require("./" + process.argv[3] + "/" + process.argv[3]);
var app = express();

app.set('port', process.argv[2]);
app.use(express.logger('dev'));
app.use(express.json());
app.use(express.urlencoded());
app.use(express.bodyParser());
app.use(express.methodOverride());
app.use(app.router);

app.post('/', function (req, res) {

	console.log('Processing Request');

	console.log('Request: ' + req);

	console.log('Request Body State: ' + req.body.state);

	console.log('Request Body Player: ' + req.body.player);

    //var game = JSON.parse(req.body);
    var game = req.body;
    var actions = agent.applyAI(game.state, game.player);
    res.send(actions);
    res.end();
});

app.get('/', function (req, res) {
    res.send('hello');
    res.end();
});

var server = http.createServer(app).listen(app.get('port'), function () {
    console.log('Express server listening on port ' + app.get('port'));
});

process.stdin.on('data', function (data) {
    var game = JSON.parse(data);
    moves = getMoves(game.state, game.player);
    console.log(JSON.stringify(moves));
});
