using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Weight { get; set; }

        public int OrganisationId { get; set; }
        [Required]
        public virtual Organisation Organisation { get; set; }
    }
}
