using CommonData.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class VenueDTO
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

        public ICollection<VenueImageDTO> Images { get; set; }

        public int EventId { get; set; }

        public VenueDTO() {
            Images = new List<VenueImageDTO>();
        }

        public VenueDTO(VenueDTO otherVenue)
        {
            Id = otherVenue.Id;
            Name = otherVenue.Name;
            Address = otherVenue.Address;
            Description = otherVenue.Description;
            GuestLimit = otherVenue.GuestLimit;
            LocationX = otherVenue.LocationX;
            LocationY = otherVenue.LocationY;
            EventId = otherVenue.EventId;
            Images = otherVenue.Images;
        }

        public static explicit operator VenueDTO(Venue venue) => new VenueDTO
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            Description = venue.Description,
            GuestLimit = venue.GuestLimit,
            LocationX = venue.LocationX,
            LocationY = venue.LocationY,
            EventId = venue.EventId
        };
        public static explicit operator Venue(VenueDTO dto) => new Venue
        {
            Name = dto.Name,
            Address = dto.Address,
            Description = dto.Description,
            GuestLimit = dto.GuestLimit,
            LocationX = dto.LocationX,
            LocationY = dto.LocationY,
            EventId = dto.EventId
        };
    }
}
