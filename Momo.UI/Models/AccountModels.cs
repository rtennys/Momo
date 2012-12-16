using System;
using System.ComponentModel.DataAnnotations;

namespace Momo.UI.Models
{
    public class RegisterExternalLoginModel
    {
        [Required, Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required, Display(Name = "Current password"), DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required, Display(Name = "New password"), StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6), DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm new password"), Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match."), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required, Display(Name = "User name")]
        public string UserName { get; set; }

        [Required, Display(Name = "Password"), DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required, Display(Name = "User name")]
        public string UserName { get; set; }

        [Required, Display(Name = "Password"), StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6), DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password"), Compare("Password", ErrorMessage = "The password and confirmation password do not match."), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
