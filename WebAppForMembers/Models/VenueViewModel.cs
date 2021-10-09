using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonData.Entities;

namespace WebAppForMembers.Models
{
    public class VenueViewModel
    {
        public Venue Venue { get; set; }
        public IList<int> ImageIds { get; set; }
    }
}
