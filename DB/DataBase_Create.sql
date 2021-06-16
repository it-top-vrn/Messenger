
create table table_USER             
(
	NICKNAME integer,
	PASSWORD varchar(256) not null
);

create unique index table_USER_NICKNAME_uindex
	on table_USER (NICKNAME);

alter table table_USER
	add constraint table_USER_pk
		primary key (NICKNAME);

alter table table_USER modify NICKNAME integer auto_increment;


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

