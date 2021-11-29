using CommonData.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class OrganisationDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        [Required]
        public TypeOfStructure TypeOfStructure { get; set; }
        [Required]
        public bool PermitNewMembers { get; set; }

        public int AdminId { get; set; }

        public List<JobDTO> Jobs { get; set; }
        public List<ProjectDTO> Projects { get; set; }
        public List<AcceptedEmailDomainDTO> AcceptedEmailDomains { get; set; }

        public OrganisationDTO()
        {
            Jobs = new List<JobDTO>();
            Projects = new List<ProjectDTO>();
            AcceptedEmailDomains = new List<AcceptedEmailDomainDTO>();
        }

        public static explicit operator OrganisationDTO(Organisation o) => new OrganisationDTO
        {
            Id = o.Id,
            Name = o.Name,
            Description = o.Description,
            TypeOfStructure = o.TypeOfStructure,
            PermitNewMembers = o.PermitNewMembers,
            AdminId = o.AdminId,
            Address = o.Address,
        };
        public static explicit operator Organisation(OrganisationDTO dto) => new Organisation
        {
            Name = dto.Name,
            Description = dto.Description,
            TypeOfStructure = dto.TypeOfStructure,
            PermitNewMembers = dto.PermitNewMembers, 
            AdminId = dto.AdminId,
            Address = dto.Address
        };

        
        public override bool Equals(object obj)
        {
            OrganisationDTO o = obj as OrganisationDTO;
            return o != null &&
                Id == o.Id &&
                Name == o.Name &&
                Description == o.Description &&
                TypeOfStructure == o.TypeOfStructure &&
                PermitNewMembers == o.PermitNewMembers &&
                AdminId == o.AdminId &&
                Address == o.Address;
        }
    }
}
