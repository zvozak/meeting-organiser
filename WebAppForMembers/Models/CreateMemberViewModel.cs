using CommonData.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppForMembers.Models
{
    public class CreateMemberViewModel
    {
        public string OrganisationName { get; set; }
        public int OrganisationId { get; set; }
        public TypeOfStructure OrganisationTypeOfStructure { get; set; }
        public IList<Job> PossibleJobsAtOrganisation { get; set; }
        public IList<Project> PossibleProjectsAtOrganisation { get; set; }
        public IList<int> IdsOfSelectedProjects { get; set; }
        public HashSet<string> OtherProjectNames { get; set; }
        [MaxLength(60)]
        [Remote(
            action: "VerifyProject",
            controller: "JoinOrganisation",
            AdditionalFields = "PossibleJobsAtOrganisation,OtherProjectNames",
            ErrorMessage ="This role has already been added or is in the dropdown list.")]
        public string OtherProjectName { get; set; }
        public int IdOfSelectedJob { get; set; }
        [MaxLength(60)]
        public string OtherJobTitle { get; set; }
        [MaxLength(60)]
        public string Department { get; set; }
        [MaxLength(60)]
        public string NameOfBoss { get; set; }
        [EmailAddress(ErrorMessage = "Az e-mail cím nem megfelelő formátumú.")]
        [DataType(DataType.EmailAddress)]
        [Remote(
            action: "VerifyBoss",
            controller: "JoinOrganisation",
            AdditionalFields = "NameOfBoss,OrganisationName",
            ErrorMessage = "Cannot find superior with matching name and email address.")]
        public string EmailOfBoss { get; set; }
        [EmailAddress(ErrorMessage = "Az e-mail cím nem megfelelő formátumú.")]
        [DataType(DataType.EmailAddress)]
        [Remote(
            action: "VerifyEmail",
            controller: "JoinOrganisation",
            AdditionalFields = "OrganisationName",
            ErrorMessage = "Email is either already used by a member or does not belong to the domain of the organisation.")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Date of joining is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfJoining { get; set; }

    }
}
