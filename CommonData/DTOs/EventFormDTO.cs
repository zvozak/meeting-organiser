using CommonData.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class EventFormDTO
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
        [Required]
        public int MemberId { get; set; }
        [Required]
        public int EventId { get; set; }

        public static explicit operator EventForm(EventFormDTO dto) => new EventForm
        {
            WantToJoin = dto.WantToJoin,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            SelectedVenueId = dto.SelectedVenueId,
            Comment = dto.Comment,
            IsFixGuest = dto.IsFixGuest,
            MemberId = dto.MemberId,
            EventId = dto.EventId
        };
        public static explicit operator EventFormDTO(EventForm form) => new EventFormDTO
        {
            WantToJoin = form.WantToJoin,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            SelectedVenueId = form.SelectedVenueId,
            Comment = form.Comment,
            IsFixGuest = form.IsFixGuest,
            MemberId = form.MemberId,
            EventId = form.EventId
        };
    }
}
