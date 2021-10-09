using CommonData.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppForMembers.Models
{
    public class EventFormViewModel
    {
        [Required]
        public bool WantToJoin { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Remote(
            action: "VerifyStartDate",
            controller: "Events",
            AdditionalFields = "EndDate,EventStartDate,EventEndDate",
            ErrorMessage = "Please choose a date from the interval below. The day of joining cannot be later than the date of leaving the event.")]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Remote(
            action: "VerifyEndDate",
            controller: "Events",
            AdditionalFields = "StartDate,EventStartDate,EventEndDate",
            ErrorMessage = "Please choose a date from the interval below. The day of joining cannot be later than the date of leaving the event.")]
        public DateTime EndDate { get; set; }
        public int? SelectedVenueId { get; set; }
        [MaxLength(500), DataType(DataType.MultilineText)]
        public string? Comment { get; set; }
        public bool IsFixGuest { get; set; }

        public int MemberId { get; set; }
        public int EventId { get; set; }
        public IList<Venue> Venues { get; set; }

        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
    }
}
