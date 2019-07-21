using System;
using System.Linq;
using Momo.Domain.Entities;

namespace Momo.Domain.ShoppingLists
{
    public interface IShoppingListItemService
    {
        DomainResult AddShoppingListItem(int shoppingListId, string name, int quantity, int aisle);
        DomainResult EditShoppingListItem(int shoppingListId, int shoppingListItemId, string name, int quantity, int aisle, decimal price);
        void DeleteShoppingListItem(int shoppingListItemId);
    }

    public sealed class ShoppingListItemService : IShoppingListItemService
    {
        public ShoppingListItemService(IRepository repository)
        {
            _repository = repository;
        }

        private readonly IRepository _repository;

        public DomainResult AddShoppingListItem(int shoppingListId, string name, int quantity, int aisle)
        {
            var result = new DomainResult();

            var shoppingList = _repository.Get<ShoppingList>(shoppingListId);
            var item = shoppingList.GetOrAddItem((name ?? "").Trim());

            item.Picked = false;
            item.Quantity = Math.Max(1, quantity);
            item.Aisle = aisle;

            return result;
        }

        public DomainResult EditShoppingListItem(int shoppingListId, int shoppingListItemId, string name, int quantity, int aisle, decimal price)
        {
            name = (name ?? "").Trim();

            var result = new DomainResult();

            var shoppingList = _repository.Get<ShoppingList>(shoppingListId);
            var item = shoppingList.ShoppingListItems.Single(x => x.Id == shoppingListItemId);

            if (!string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase) && shoppingList.ShoppingListItems.Any(x => x.Id != shoppingListItemId && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
                return result.Add("Name", "Items must be unique");

            item.Name = name;
            item.Quantity = Math.Max(0, quantity);
            item.Aisle = aisle;
            item.Price = price;

            result.Data.ShoppingList = shoppingList;

            return result;
        }

        public void DeleteShoppingListItem(int shoppingListItemId)
        {
            _repository.Remove<ShoppingListItem>(shoppingListItemId);
        }
    }
}
