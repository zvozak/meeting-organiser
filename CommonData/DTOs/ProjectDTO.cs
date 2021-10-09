using CommonData.Entities;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class ProjectDTO
    {
        [Key]
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }

        public static explicit operator ProjectDTO(Project p) => new ProjectDTO
        {
            Id = p.Id,
            OrganisationId = p.OrganisationId,
            Name = p.Name,
            Weight = p.Weight
        };
        public static explicit operator Project(ProjectDTO dto) => new Project
        {
            OrganisationId = dto.OrganisationId,
            Name = dto.Name,
            Weight = dto.Weight
        };
    }
}
