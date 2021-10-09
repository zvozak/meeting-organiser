using System;
using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public DateTime DeadlineForApplication { get; set; }


        [Required]
        public int GuestLimit { get; set; }

        [Required]
        public bool IsConnectedGraphRequired { get; set; }

        [Required]
        public bool IsWeightRequired { get; set; }
        public int ProjectImportanceWeight { get; set; }
        public int NumberOfProjectsWeight { get; set; }

        public int NumberOfSubordinatesWeight { get; set; }

        public int NumberOfNeighboursWeight { get; set; }

        public int JobWeight { get; set; }


        public int OrganisationId { get; set; }

        [Required]
        public virtual Organisation Organisation { get; set; }
    }
}
