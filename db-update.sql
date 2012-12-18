use Momo;

if not exists (select * from sys.objects where object_id = object_id(N'[dbo].[ShoppingList]') and type in (N'U'))
begin
    create table dbo.ShoppingList (
        Id int identity(1,1) not null constraint PK_ShoppingList primary key,
        Version int not null,
        Name nvarchar(255) not null,
        UserProfileId int not null constraint FK_ShoppingList_UserProfileId foreign key references UserProfile (Id)
    );
end;

GO
