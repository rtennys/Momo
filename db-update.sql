use Momo;

if exists (select * from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME = N'Isle' and TABLE_NAME = N'ShoppingListItem')
begin
    exec sp_rename 'dbo.ShoppingListItem.Isle', 'Aisle', 'COLUMN';
end;

GO
