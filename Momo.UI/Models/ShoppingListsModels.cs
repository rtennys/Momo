using System;
using System.ComponentModel.DataAnnotations;

namespace Momo.UI.Models
{
    public class ShoppingListsAddModel
    {
        [Required]
        public string Name { get; set; }
    }
}
