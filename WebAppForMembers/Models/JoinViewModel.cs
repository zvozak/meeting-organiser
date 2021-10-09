using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppForMembers.Models
{
    public class JoinViewModel
    {
        [Required(ErrorMessage ="No organisations have been selected for joining.")]
        [Remote(
            action: "VerifyOrganisation",
            controller: "JoinOrganisation",
            ErrorMessage ="Organisation is not found or You are already a member of it.")]
        public String OrganisationName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage ="Not a valid email address.")]
        [Remote(
            action: "VerifyMemberBy", 
            controller: "JoinOrganisation", 
            AdditionalFields ="OrganisationName", 
            ErrorMessage = "Not a valid email address for organisation OR already joined organisation.")]
        public String EmailAddressAtOrganisation { get; set; }
    }
}
