using System;

namespace Momo.Domain.Entities
{
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
