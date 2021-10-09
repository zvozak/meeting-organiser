using System;

namespace MeetingOrganiserDesktopApp.Model
{
    public class VenueEventArgs : EventArgs
    {
        public int EventId { get; set; }
        public int VenueId { get; set; }
    }
}