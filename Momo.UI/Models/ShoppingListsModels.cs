using System;
using System.ComponentModel.DataAnnotations;

namespace Momo.UI.Models
{
    public class ShoppingListsIndexModel
    {
        public bool ShowNew { get; set; }
        public string[] ShoppingLists { get; set; }
    }

    public class ShoppingListsShowModel
    {
        public string Name { get; set; }
    }

    public class ShoppingListsAddModel
    {
        [Required]
        public string Name { get; set; }
    }
}
