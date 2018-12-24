using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IList<UserProfile> _sharedWith = new List<UserProfile>();

        public virtual UserProfile UserProfile { get; protected set; }
        public virtual string Name { get; protected internal set; }

        public virtual IReadOnlyList<ShoppingListItem> ShoppingListItems
        {
            get { return _shoppingListItems.AsReadOnly(); }
        }

        public virtual IReadOnlyList<UserProfile> SharedWith
        {
            get { return _sharedWith.AsReadOnly(); }
        }

        public virtual void StartSharing(UserProfile user)
        {
            _sharedWith.Add(user);
            user.AddSharedList(this);
        }

        public virtual void StopSharing(string username)
        {
            var user = _sharedWith.Single(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));
            _sharedWith.Remove(user);
            user.RemoveSharedList(this);
        }

        protected internal virtual ShoppingListItem GetOrAddItem(string name)
        {
            var item = ShoppingListItems.SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new ShoppingListItem(this, name.ToTitleCase());
                _shoppingListItems.Add(item);
            }

            return item;
        }

        protected internal virtual void Clear(bool checkedOnly)
        {
            foreach (var item in _shoppingListItems.Where(x => !checkedOnly || x.Picked))
            {
                item.Quantity = 0;
                item.Picked = false;
            }
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
        public virtual int Aisle { get; set; }
        public virtual decimal Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual bool Picked { get; set; }
    }
}
