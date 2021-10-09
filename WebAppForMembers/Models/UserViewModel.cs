using CommonData.DTOs;
using CommonData.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppForMembers.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(60, ErrorMessage = "Maximum 60 karakter lehet.")]
        public String GuestName { get; set; }

        [Required(ErrorMessage = "Az e-mail cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Az e-mail cím nem megfelelő formátumú.")]
        [DataType(DataType.EmailAddress)]
        public String GuestEmail { get; set; }

        [Required(ErrorMessage = "A cím megadása kötelező.")]
        public String GuestAddress { get; set; }

        [Required(ErrorMessage = "A telefonszám megadása kötelező.")]
        [Phone(ErrorMessage = "A telefonszám formátuma nem megfelelő.")]
        [DataType(DataType.PhoneNumber)]
        public String GuestPhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [BindRequired]
        public IList< SpecialNeedDTO> SpecialNeeds { get; set; }
        public static IList<String> NamesOfSpecialNeeds = new List<String>(Enum.GetNames(typeof(SpecialNeed)));
    }
}