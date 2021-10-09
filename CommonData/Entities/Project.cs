using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }

        [Required]
        public virtual Organisation Organisation { get; set; }
    }
}
