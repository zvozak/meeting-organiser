using System;
using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Weight { get; set; }

        public int OrganisationId { get; set; }
        [Required]
        public virtual Organisation Organisation { get; set; }

        public override Boolean Equals(Object obj)
        {
            Job job = obj as Job;
            return job != null &&
                OrganisationId == job.OrganisationId && 
                Title == job.Title;
        }

    }
}
