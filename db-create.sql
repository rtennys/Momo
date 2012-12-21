use master;

if exists (select * from sys.databases where name = N'Momo')
begin
    exec msdb.dbo.sp_delete_database_backuphistory @database_name = N'Momo';
    alter database Momo set single_user with rollback immediate;
    drop database Momo;
end

create database Momo;

GO



use Momo;

create table dbo.UserProfile (
    Id int identity(1,1) not null constraint PK_UserProfile primary key,
    Version int not null,
    Username nvarchar(255) not null
);

create table dbo.ShoppingList (
    Id int identity(1,1) not null constraint PK_ShoppingList primary key,
    Version int not null,
    Name nvarchar(255) not null,
    UserProfileId int not null constraint FK_ShoppingList_UserProfileId foreign key references UserProfile (Id)
);

create table dbo.ShoppingListItem (
    Id int identity(1,1) not null constraint PK_ShoppingListItem primary key,
    Version int not null,
    Name nvarchar(255) not null,
    Isle int not null,
    Price decimal(9,2) not null,
    Quantity int not null,
    Picked bit not null,
    ShoppingListId int not null constraint FK_ShoppingListItem_ShoppingListId foreign key references ShoppingList (Id)
);

GO
