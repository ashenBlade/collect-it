create schema accounts;

create table accounts.subscriptions (
    id serial primary key,
    name varchar(50) not null,
    description varchar not null,
    duration int not null,
    price int not null,
    constraint price_not_negative check (price >= 0),
    constraint days_duration_positive check (duration > 0)
);

create table accounts.roles (
    id serial primary key,
    name varchar not null,
    normalized_name varchar generated always as ( upper(name) ) stored
);

create table accounts.users (
    id serial primary key,
    name varchar not null,
    normalized_name varchar generated always as ( upper(name) ) stored,
    email varchar not null,
    normalized_email varchar generated always as ( upper(email) ) stored,
    password_hash varchar not null
);

-- For equality comparison for 'user_id' in gist
create extension btree_gist;

create table accounts.users_subscriptions (
    id serial primary key,
    user_id int references accounts.users(id) not null,
    subscription_id int references accounts.subscriptions(id) not null,
    during daterange not null,
    constraint max_1_subscription_per_user_at_time exclude using gist(user_id with =, during with &&)
);

create function fill_during_for_user_subscription_func() returns trigger as $$
    begin
        if (tg_op = 'INSERT' and new.during is null) then
            new.during := daterange(current_date, current_date + (select duration from accounts.subscriptions as s where s.id = new.subscription_id));
        end if;
        return new;
    end
    $$ language plpgsql;


create trigger fill_during_for_user_subscription
    before insert on accounts.users_subscriptions
    for each row
    execute function fill_during_for_user_subscription_func();
