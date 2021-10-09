using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class VenueImage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public byte[] ImageSmall { get; set; }
        [Required]
        public byte[] ImageLarge { get; set; }
        public int VenueId { get; set; }

        [Required]
        public virtual Venue Venue { get; set; }
    }
}
