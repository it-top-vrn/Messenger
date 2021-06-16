create table table_user
(
	nickname varchar(20) not null,
	password varchar(256) not null
);

create unique index table_user_nickname_uindex
	on table_user (nickname);

alter table table_user
	add constraint table_user_pk
		primary key (nickname);
