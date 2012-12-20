using System;
using System.Collections.Generic;
using Momo.Common;

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

        public virtual string Username { get; protected set; }

        public virtual IList<ShoppingList> ShoppingLists
        {
            get { return _shoppingLists.AsReadOnly(); }
        }

        public virtual void CreateShoppingList(string name)
        {
            _shoppingLists.Add(new ShoppingList(this, name));
        }
    }
}
