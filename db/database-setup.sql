create database collect_it_db;

create role reader
    encrypted password 'xU80wqW34'
    login
    noinherit
    nocreaterole
    nocreatedb
    nosuperuser
    noreplication;

create role administrator
    encrypted password 'o25Yvs02re'
    login
    noinherit
    superuser
    createrole
    replication;

grant connect on database collect_it_db to reader;
revoke public from reader;