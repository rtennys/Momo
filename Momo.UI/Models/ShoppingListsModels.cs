using System;
using Momo.Domain.Commands;
using Momo.Domain.Entities;

namespace Momo.UI.Models
{
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

    public class ShoppingListItemModel
    {
        public ShoppingListItemModel(ShoppingListItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Aisle = item.Aisle;
            Quantity = item.Quantity;
            Price = item.Price;
            Picked = item.Picked;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Aisle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool Picked { get; set; }
    }
}
