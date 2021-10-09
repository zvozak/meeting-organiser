using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class AcceptedEmailDomain
    {
        [Key]
        public int OrganisationId { get; set; }
        [Key]
        public string DomainName { get; set; }

        [Required]
        public virtual Organisation Organisation { get; set; }
    }
}
