using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppForMembers.Models
{
    public class EventViewModel
    {
        public enum EventState
        {
            Unseen,
            Accepted,
            Rejected
        }

        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DeadlineForApplication { get; set; }
        public int EventId { get; set; }
        public string OrganisationName { get; set; }
        public EventState State {get; set;}
    }
}
