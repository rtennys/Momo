using System;
using System.ComponentModel.DataAnnotations;

namespace Momo.Domain.Users
{
    public class AddUserModel
    {
        [Required, RegularExpression(@"^[A-Za-z]+[A-Za-z0-9-]*$", ErrorMessage = "Start with a letter and then letters, numbers, and dashes only")]
        public string Username { get; set; }
    }
}
