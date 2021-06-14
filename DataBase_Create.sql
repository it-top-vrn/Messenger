create table table_USER
(
	ID integer auto_increment,
	NICKNAME varchar(256) not null,
	PASSWORD varchar(256) not null,
	constraint table_USER_pk
		primary key (ID)
);

create unique index table_USER_NICKNAME_uindex
	on table_USER (NICKNAME);







create table table_MESSAGE
(
	senderID varchar(256) not null,
	recipientID varchar(256) not null,
	Message_Time varchar(256) not null,
	MESSAGE text not null
);

create unique index table_MESSAGE_recipientID_uindex
	on table_MESSAGE (recipientID);

create unique index table_MESSAGE_senderID_uindex
	on table_MESSAGE (senderID);

