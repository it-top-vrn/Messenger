
create table table_user
(
	nickname varchar(20),
	password varchar(256) not null
);

create unique index table_USER_NICKNAME_uindex
	on table_USER (NICKNAME);

alter table table_USER
	add constraint table_USER_pk
		primary key (NICKNAME);

alter table table_USER modify NICKNAME integer auto_increment;

