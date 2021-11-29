using CommonData.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{

    public class EventDTO
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

        public List<VenueDTO> Venues { get; set; }

        public int ProjectImportanceWeight { get; set; }
        public int NumberOfProjectsWeight { get; set; }

        public int NumberOfSubordinatesWeight { get; set; }

        public int NumberOfNeighboursWeight { get; set; }

        public int JobWeight { get; set; }
        [Required]
        public int GuestLimit { get; set; }

        [Required]
        public bool IsConnectedGraphRequired { get; set; }
       
        [Required]
        public bool IsWeightRequired { get; set; }
       


        public int OrganisationId { get; set; }

        public EventDTO(EventDTO otherEvent)
        {
            OrganisationId = otherEvent.OrganisationId;
            Id = otherEvent.Id;
            Name = otherEvent.Name;
            Description = otherEvent.Description;
            StartDate = otherEvent.StartDate;
            EndDate = otherEvent.EndDate;
            DeadlineForApplication = otherEvent.DeadlineForApplication;
            GuestLimit = otherEvent.GuestLimit;
            IsConnectedGraphRequired = otherEvent.IsConnectedGraphRequired;
            IsWeightRequired = otherEvent.IsWeightRequired;
            ProjectImportanceWeight = otherEvent.ProjectImportanceWeight;
            NumberOfProjectsWeight = otherEvent.NumberOfProjectsWeight;
            NumberOfSubordinatesWeight = otherEvent.NumberOfSubordinatesWeight;
            NumberOfNeighboursWeight = otherEvent.NumberOfNeighboursWeight;
            JobWeight = otherEvent.JobWeight;
        }



        public EventDTO()
        {
            Venues = new List<VenueDTO>();
        }

        public static explicit operator Event(EventDTO dto) => new Event
        {
            OrganisationId = dto.OrganisationId,
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            DeadlineForApplication = dto.DeadlineForApplication,
            GuestLimit = dto.GuestLimit,
            IsConnectedGraphRequired = dto.IsConnectedGraphRequired,
            IsWeightRequired = dto.IsWeightRequired,
            ProjectImportanceWeight = dto.ProjectImportanceWeight,
            NumberOfProjectsWeight = dto.NumberOfProjectsWeight,
            NumberOfSubordinatesWeight = dto.NumberOfSubordinatesWeight,
            NumberOfNeighboursWeight = dto.NumberOfNeighboursWeight,
            JobWeight = dto.JobWeight
        };
        public static explicit operator EventDTO(Event e) => new EventDTO
        {
            OrganisationId = e.OrganisationId,
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            DeadlineForApplication = e.DeadlineForApplication,
            GuestLimit = e.GuestLimit,
            IsConnectedGraphRequired = e.IsConnectedGraphRequired,
            IsWeightRequired = e.IsWeightRequired,
            ProjectImportanceWeight = e.ProjectImportanceWeight,
            NumberOfProjectsWeight = e.NumberOfProjectsWeight,
            NumberOfSubordinatesWeight = e.NumberOfSubordinatesWeight,
            NumberOfNeighboursWeight = e.NumberOfNeighboursWeight,
            JobWeight = e.JobWeight
        };

        public override bool Equals(object obj)
        {
            EventDTO e = obj as EventDTO;
            return e != null &&
                OrganisationId == e.OrganisationId &&
                Id == e.Id &&
                Name == e.Name &&
                Description == e.Description &&
                StartDate == e.StartDate &&
                EndDate == e.EndDate &&
                DeadlineForApplication == e.DeadlineForApplication &&
                GuestLimit == e.GuestLimit &&
                IsConnectedGraphRequired == e.IsConnectedGraphRequired &&
                IsWeightRequired == e.IsWeightRequired &&
                ProjectImportanceWeight == e.ProjectImportanceWeight &&
                NumberOfProjectsWeight == e.NumberOfProjectsWeight &&
                NumberOfSubordinatesWeight == e.NumberOfSubordinatesWeight &&
                NumberOfNeighboursWeight == e.NumberOfNeighboursWeight &&
                JobWeight == e.JobWeight;
        }
    }
}
