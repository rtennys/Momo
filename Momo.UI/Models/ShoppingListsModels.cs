using System;
using System.Web.Mvc;
using Momo.Domain.Commands;
using Momo.Domain.Entities;

namespace Momo.UI.Models
{
    public class ShoppingListsIndexModel
    {
        public ShoppingListModel[] ShoppingLists { get; set; }
        public ShoppingListModel[] SharedLists { get; set; }
    }

    public class ShoppingListsShowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ShoppingListsRenameModel : RenameShoppingListCommand
    {
    }

    public class ShoppingListModel
    {
        public ShoppingListModel(ShoppingList list, UrlHelper urlHelper)
        {
            Name = list.Name;
            Url = urlHelper.Action("Show", "ShoppingLists", new {username = list.UserProfile.Username, shoppinglist = list.Name});
        }

        public string Name { get; set; }
        public string Url { get; set; }
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
