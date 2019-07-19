using System;
using System.Linq;
using Momo.Domain.Entities;

namespace Momo.Domain.ShoppingLists
{
    public interface IShoppingListService
    {
        DomainResult AddShoppingList(string username, string name);
        DomainResult RenameShoppingList(string username, int id, string name);
        DomainResult DeleteShoppingList(string username, int id);
        DomainResult ClearShoppingList(string username, int id, bool checkedOnly);
    }

    public sealed class ShoppingListService : IShoppingListService
    {
        public ShoppingListService(IRepository repository)
        {
            _repository = repository;
        }

        private readonly IRepository _repository;

        public DomainResult AddShoppingList(string username, string name)
        {
            var result = new DomainResult();

            var user = _repository.Get<UserProfile>(x => x.Username == username);

            if (user.ShoppingLists.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
                result.Add("Name", "Name Must Be Unique");
            else
                result.Data.ShoppingList = user.AddShoppingList(name);

            return result;
        }

        public DomainResult RenameShoppingList(string username, int id, string name)
        {
            var result = new DomainResult();

            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.SingleOrDefault(x => x.Id == id);

            if (shoppingList == null)
                result.Add("Id", "Shopping List Not Found");
            else if (!string.Equals(shoppingList.Name, name, StringComparison.OrdinalIgnoreCase) && user.ShoppingLists.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
                result.Add("Name", "Name Must Be Unique");
            else
                shoppingList.Name = name;

            return result;
        }

        public DomainResult DeleteShoppingList(string username, int id)
        {
            var result = new DomainResult();

            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.SingleOrDefault(x => x.Id == id);

            if (shoppingList == null)
                result.Add("Id", "Shopping List Not Found");
            else
                user.Remove(shoppingList);

            return result;
        }

        public DomainResult ClearShoppingList(string username, int id, bool checkedOnly)
        {
            var result = new DomainResult();

            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.SingleOrDefault(x => x.Id == id);

            if (shoppingList == null)
                result.Add("Id", "Shopping List Not Found");
            else
                shoppingList.Clear(checkedOnly);

            return result;
        }
    }
}
