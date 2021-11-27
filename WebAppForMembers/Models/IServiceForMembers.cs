using CommonData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppForMembers.Models
{
    public interface IServiceForMembers
    {
        public bool SaveChanges();

        #region Queries
        public Member GetMember(string organisationName, string realUserName, string userEmail);
        public IEnumerable<Organisation> GetOrganisationsOf(int userId);
        public IEnumerable<EventViewModel> GetAvailableEventsOf(int userId);
        public Organisation GetOrganisation(int organisationId);
        public IEnumerable<EventViewModel> GetEventsOf(int organisationId);
        public EventViewModel GetEventViewModel(int eventId);
        public Event GetEvent(int eventId);
        public EventForm? GetEventForm(int eventId, int memberId);
        public Venue GetVenue(int venueId);
        public IEnumerable<Venue> GetVenues(int eventId);
        public IEnumerable<VenueImage> GetVenueImages(int venueId);
        public Byte[] GetVenueMainImage(int? venueId);
        public Byte[] GetVenueImage(Int32? imageId, Boolean large);
        public bool CreateOrganisation(Organisation organisation);
        public IEnumerable<int> GetVenueImageIds(int venueId);
        public DateTime? StrictestDeadlineAt(int organisationId);
        public bool IsExistingOrganisation(string organisationName);
        public bool IsMemberOfOrganisation(string organisationName, string realUserName, string userEmail);
        public bool IsMemberOfOrganisation(string organisationName, int userId);
        public Organisation GetOrganisationByName(string organisationName);
        public int GetNumberOfEventsIn(string organisationName);
        public int GetMemberIdBy(string organisationName, string realUserName, string userEmail);
        public int GetMemberIdBy(int userId, int organisationId);
        public User GetUserBy(string userName);
        public bool IsExistingMembership(int memberId, int userId);
        public bool CanJoinOrganisation(string organisationName, string email);

        public IEnumerable<Job> PossibleJobsAtOrganisation(int organisationId);
        public IEnumerable<Project> PossibleProjectsAtOrganisation(int organisationId);
        public Project GetProject(string name, int organisationId);
        public Job GetJob(string title, int organisationId);
        #endregion

        #region Create
        public bool CreateJob(string jobTitle, int organisationId);
        public bool CreateProject(string projectName, int organisationId);
        public bool CreateMemberOfProject(int memberId, int projectId);
        public bool CreateMembership(string organisationName, int userId, string userEmail);

        public bool CreateMemberships(string userName);
        public bool CreateEventForm(EventForm eventForm);

        public bool AddMember(Member member);

        public bool IsEmailAtOrganisation(string OrganisationName, string EmailAddressAtOrganisation);
        #endregion

        #region Edit

        public bool RejectEvent(int memberId, int eventId);
        public bool AcceptEvent(int memberId, int eventId);
        public bool EditEventForm(EventForm eventForm);

        #endregion

        #region Deleting
        public bool DeleteMembership(int userId, int organisationId);
        #endregion
    }
}
