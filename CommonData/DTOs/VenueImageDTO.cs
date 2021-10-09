using CommonData.Entities;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class VenueImageDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public byte[] ImageSmall { get; set; }
        [Required]
        public byte[] ImageLarge { get; set; }
        public int VenueId { get; set; }
        public static explicit operator VenueImageDTO(VenueImage image) => new VenueImageDTO
        {
            Id = image.Id,
            ImageLarge = image.ImageLarge,
            ImageSmall = image.ImageSmall,
            VenueId = image.VenueId
        };
        public static explicit operator VenueImage(VenueImageDTO dto) => new VenueImage
        {
            ImageLarge = dto.ImageLarge,
            ImageSmall = dto.ImageSmall,
            VenueId = dto.VenueId
        };
    }
}
