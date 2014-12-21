var http 		= require('http');
var querystring = require('querystring');
var url         = require('url');
var sqlite 		= require('sqlite3').verbose();

var db 			= new sqlite.Database('Database.sqlite');

function OnLogger(request, response)
{
	var postData = "";
	request.setEncoding("utf-8");
	request.addListener('data', function(data){
		postData += data;
	});
	request.addListener('end', function(){
		var postDataText = querystring.parse(postData);
		//var jsonObjs;
		//try{
		//	jsonObjs = querystring.parse(postData);
		//	console.log(json['Condition']);
		//	console.log(json['StackTrace']);
		//}
		//catch(err) {
		//	console.log(err);
		//	console.log(postDataText);
		//}
		
		db.serialize(function(){
			db.run("CREATE TABLE IF NOT EXISTS Logger (ID integer primary key autoincrement, LOGGER_DATETIME DATETIME, LOGGER_TEXT TEXT)");
			db.prepare("INSERT INTO Logger(LOGGER_DATETIME, LOGGER_TEXT) VALUES(datetime(\'now\',\'localtime\'), \"" + postDataText + "\")");
		});
	});
}

var ServerModule = {
	"/Logger":OnLogger,
};


http.createServer(function(request, response){
	var pathname = url.parse(request.url).pathname
	
	if (ServerModule[pathname])
	{
		ServerModule[pathname](request, response);
	}
	else
	{
		response.end('404');
	}
}).listen(8000);