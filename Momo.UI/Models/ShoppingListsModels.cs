using System;
using Momo.Domain.Commands;

namespace Momo.UI.Models
{
    public class ShoppingListsIndexModel
    {
        public bool ShowNew { get; set; }
        public string[] ShoppingLists { get; set; }
    }

    public class ShoppingListsShowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ShoppingListsAddModel : AddShoppingListCommand
    {
    }

    public class ShoppingListsRenameModel : RenameShoppingListCommand
    {
    }
}
