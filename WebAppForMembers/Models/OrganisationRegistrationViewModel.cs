using CommonData.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAppForMembers.Models
{
    public class OrganisationRegistrationViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public string Description { get; set; }
        [Required]
        [DisplayName ("Type of structure")]
        public TypeOfStructure TypeOfStructure { get; set; }
        [Required]
        [DisplayName("Permit people - not listed in organisation's database - to join")]
        public bool PermitNewMembers { get; set; }


        [Required(ErrorMessage = "Administrator is required.")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "The length or format of username is incorrect.")]
        [Remote (
            action: "VerifyAdminUsername",
            controller: "ForOrganisations",
            ErrorMessage = "No such username. Please provide an already registrated username as administrator.")]
        [DisplayName("Username of administrator")]
        public String AdminUserName { get; set; }

    }
}