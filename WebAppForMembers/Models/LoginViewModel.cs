using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppForMembers.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "A felhasználónév megadása kötelező.")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [DataType(DataType.Password)]        
        public String UserPassword { get; set; }

	    public Boolean RememberLogin { get; set; }
	}
}