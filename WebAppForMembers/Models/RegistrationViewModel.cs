using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppForMembers.Models
{
    public class RegistrationViewModel : UserViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "The length or format of username is incorrect.")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public String UserPassword { get; set; }

        [Required(ErrorMessage = "Repeat password.")]
        [Compare(nameof(UserPassword), ErrorMessage = "Passwords does not match.")]
        [DataType(DataType.Password)]
        public String UserConfirmPassword { get; set; }
    }
}