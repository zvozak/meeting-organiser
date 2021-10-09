using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public enum TypeOfStructure
    {
        Hierarchical,
        ProjectBased
    }

    public class Organisation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public string Description { get; set; }
        [Required]
        public TypeOfStructure TypeOfStructure { get; set; }
        [Required]
        public bool PermitNewMembers { get; set; }

        public int AdminId { get; set; }

        public virtual User Admin { get; set; }
    }
}
