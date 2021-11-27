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

        public override bool Equals(object obj)
        {
            EventViewModel e = obj as EventViewModel;
            return e != null && Object.Equals(this.Name, e.Name) && Object.Equals(this.Description, e.Description) &&
                Object.Equals(this.StartDate, e.StartDate) && Object.Equals(this.EndDate, e.EndDate) &&
                Object.Equals(this.DeadlineForApplication, e.DeadlineForApplication) && Object.Equals(this.EventId, e.EventId) &&
                Object.Equals(this.OrganisationName, e.OrganisationName) && Object.Equals(this.State, e.State);
        }
    }
}
