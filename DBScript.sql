create table messages
(
	Id varchar(50) primary key,
	[From] varchar(50),
	[To] varchar(50),
	[Text] varchar(50),
	IsSent int
);
create table events
(
	Id varchar(50) primary key,
	[Type] varchar(50),
	[Description] varchar(50),
	Organizer varchar(50),
	Subscriber varchar(50),
	IsSent int
);
create table subscribers
(
	Subscriber varchar(50),
	[Type] varchar(50)
);