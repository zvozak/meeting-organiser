using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class Venue
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Required, MaxLength(50)]
        public string Address { get; set; }
        [MaxLength(500), DataType(DataType.MultilineText)]
        public string? Description { get; set; }
        [Required]
        public int GuestLimit { get; set; }
        [Required]
        public double LocationX { get; set; }
        [Required] 
        public double LocationY { get; set; }
        public int EventId { get; set; }

        [Required]
        public virtual Event Event { get; set; }

    }
}
