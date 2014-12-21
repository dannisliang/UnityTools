## Initialize

```
var sqlite = require('sqlite3').verbose();
var db     = new sqlite.Database('Database.sqlite');  

```

## Create TABLE
```
db.run("CREATE TABLE IF NOT EXISTS $(TABLE)(
	ID integer primary key autoincrement,
	$(NAME) TEXT NOT NULL,
	$(NAME) DATETIME,
	$(NAME) DATE,
	$(NAME) TIME,
	$(NAME) BIGINT，
	$(NAME) REAL,
	$(NAME) NUMERIC)"

datetime('now', 'localtime')        当前时间
datetime('now', 'start of day')     今天开始的时间
datetime('now', 'start of monty')   当月开始的时间 
```

## Insert
```
db.prepare("INSERT INTO $TABLE($NAME1, $NAME2,...) VALUES ($VALUE1, $VALUE2, ...)");
```

## Select