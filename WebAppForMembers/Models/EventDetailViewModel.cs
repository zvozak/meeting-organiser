using CommonData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppForMembers.Models
{
    public class EventDetailViewModel
    {
        public EventViewModel Event { get; set; }
        public IList<Venue> Venues { get; set; }
    }
}
