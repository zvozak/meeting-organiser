using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonData.Entities;

namespace WebAppForMembers.Models
{
    public class OrganisationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public int NumberOfEvents { get; set; }
    }
}
