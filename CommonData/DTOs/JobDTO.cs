using CommonData.Entities;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class JobDTO
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Weight { get; set; }
        [Required]
        public int OrganisationId { get; set; }

        public static explicit operator Job(JobDTO dto) => new Job
        {
            Title = dto.Title,
            Weight = dto.Weight,
            OrganisationId = dto.OrganisationId
        };
        public static explicit operator JobDTO(Job job) => new JobDTO
        {
            Id = job.Id,
            Title = job.Title,
            Weight = job.Weight,
            OrganisationId = job.OrganisationId
        };
    }
}
