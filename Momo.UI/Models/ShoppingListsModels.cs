using System;
using Momo.Domain.Commands;
using Momo.Domain.Entities;

namespace Momo.UI.Models
{
    public class ShoppingListsIndexModel
    {
        public string[] ShoppingLists { get; set; }
    }

    public class ShoppingListsShowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ShoppingListItem[] Items { get; set; }
    }

    public class ShoppingListsAddModel : AddShoppingListCommand
    {
    }

    public class ShoppingListsRenameModel : RenameShoppingListCommand
    {
    }

    public class ShoppingListsAddItemModel : AddShoppingListItemCommand
    {
    }

    public class ShoppingListsEditItemModel : EditShoppingListItemCommand
    {
    }
}
