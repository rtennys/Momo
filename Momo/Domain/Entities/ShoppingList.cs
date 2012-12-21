using System;
using System.Collections.Generic;
using System.Linq;
using Momo.Common;

namespace Momo.Domain.Entities
{
    public class ShoppingList : EntityBase
    {
        protected ShoppingList()
        {
        }

        internal ShoppingList(UserProfile userProfile, string name)
        {
            UserProfile = userProfile;
            Name = name;
        }

        private readonly IList<ShoppingListItem> _shoppingListItems = new List<ShoppingListItem>();

        public virtual UserProfile UserProfile { get; protected set; }
        public virtual string Name { get; protected internal set; }

        public virtual IReadOnlyList<ShoppingListItem> ShoppingListItems
        {
            get { return _shoppingListItems.AsReadOnly(); }
        }

        protected internal virtual ShoppingListItem GetOrAddItem(string name)
        {
            var item = ShoppingListItems.SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new ShoppingListItem(this, name);
                _shoppingListItems.Add(item);
            }

            return item;
        }
    }

    public class ShoppingListItem : EntityBase
    {
        protected ShoppingListItem()
        {
        }

        public ShoppingListItem(ShoppingList shoppingList, string name)
        {
            ShoppingList = shoppingList;
            Name = name;
        }

        public virtual ShoppingList ShoppingList { get; protected set; }
        public virtual string Name { get; protected internal set; }
        public virtual int Isle { get; set; }
        public virtual decimal Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual bool Picked { get; set; }
    }
}
