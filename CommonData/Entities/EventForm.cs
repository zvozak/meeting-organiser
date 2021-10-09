using System;
using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class EventForm
    {
        [Required]
        public bool WantToJoin { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        public int? SelectedVenueId { get; set; }
        [MaxLength(500), DataType(DataType.MultilineText)]
        public string? Comment { get; set; }
        public bool IsFixGuest { get; set; }

        public int MemberId { get; set; }
        public int EventId { get; set; }

        [Required]
        public virtual Member Member { get; set; }
        [Required]
        public virtual Event Event { get; set; }
        public virtual Venue SelectedVenue { get; set; }
    }
}
