using System;
using System.Collections.Generic;

namespace Momo.Domain.Entities
{
    public class UserProfile : EntityBase
    {
        protected UserProfile()
        {
        }

        public UserProfile(string username)
        {
            Username = username;
        }

        private readonly IList<ShoppingList> _shoppingLists = new List<ShoppingList>();
        private readonly IList<ShoppingList> _sharedLists = new List<ShoppingList>();

        public virtual string Username { get; protected set; }

        public virtual IReadOnlyList<ShoppingList> ShoppingLists
        {
            get { return _shoppingLists.AsReadOnly(); }
        }

        public virtual IReadOnlyList<ShoppingList> SharedLists
        {
            get { return _sharedLists.AsReadOnly(); }
        }

        protected internal virtual ShoppingList AddShoppingList(string name)
        {
            var list = new ShoppingList(this, name);
            _shoppingLists.Add(list);
            return list;
        }

        protected internal virtual void Remove(ShoppingList shoppingList)
        {
            _shoppingLists.Remove(shoppingList);
        }

        protected internal virtual void AddSharedList(ShoppingList shoppingList)
        {
            _sharedLists.Add(shoppingList);
        }

        protected internal virtual void RemoveSharedList(ShoppingList shoppingList)
        {
            _sharedLists.Remove(shoppingList);
        }
    }
}
