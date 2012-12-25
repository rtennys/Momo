use Momo;

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'ShoppingList')
begin
    create table dbo.ShoppingList (
        Id int identity(1,1) not null constraint PK_ShoppingList primary key,
        Version int not null,
        Name nvarchar(255) not null,
        UserProfileId int not null constraint FK_ShoppingList_UserProfileId foreign key references UserProfile (Id)
    );
end;

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'ShoppingListItem')
begin
    create table dbo.ShoppingListItem (
        Id int identity(1,1) not null constraint PK_ShoppingListItem primary key,
        Version int not null,
        Name nvarchar(255) not null,
        Price decimal(9,2) not null,
        Quantity int not null,
        Picked bit not null,
        ShoppingListId int not null constraint FK_ShoppingListItem_ShoppingListId foreign key references ShoppingList (Id)
    );
end;

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = N'ShoppingListItem' and COLUMN_NAME = N'Isle')
begin
    alter table dbo.ShoppingListItem add Isle int not null default (0);
end;

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = N'ShoppingListToUserProfile')
begin
	create table dbo.ShoppingListToUserProfile (
		ShoppingListId int not null constraint FK_ShoppingListToUserProfile_ShoppingListId foreign key references ShoppingList (Id),
		UserProfileId int not null constraint FK_ShoppingListToUserProfile_UserProfileId foreign key references UserProfile (Id)
	);
end;

GO
