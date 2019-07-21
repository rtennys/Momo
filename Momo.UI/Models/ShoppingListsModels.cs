using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Momo.Domain.Entities;
using Momo.UI.Controllers;

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
        public IList<ShoppingListItemModel> ListItems { get; set; }

        public string Css(IGrouping<int, ShoppingListItemModel> lookup)
        {
            return string.Join(" ", GetCss(lookup));
        }

        private IEnumerable<string> GetCss(IGrouping<int, ShoppingListItemModel> lookup)
        {
            if (lookup.All(x => x.Css.Length > 0)) yield return "nothing-needed";
            if (lookup.All(x => x.Css == "zero")) yield return "zero";
            if (lookup.All(x => x.Css == "picked")) yield return "picked";
        }
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

        public string Css
        {
            get { return string.Join(" ", GetCss()); }
        }

        private IEnumerable<string> GetCss()
        {
            if (Picked) yield return "picked";
            if (Quantity == 0) yield return "zero";
        }
    }
}
