using CommonData.Entities;
using System.Collections.Generic;

namespace WebAppForMembers.Models
{
    public class OrganisationDetailViewModel
    {
        public Organisation Organisation { get; set; }
        public IList<EventViewModel> Events { get; set; }

    }
}
