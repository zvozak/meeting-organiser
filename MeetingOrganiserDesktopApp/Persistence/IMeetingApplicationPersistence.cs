using CommonData.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingOrganiserDesktopApp.Persistence
{
    public interface IMeetingApplicationPersistence
    {

        #region Reading
        public  OrganisationDTO ReadOrganisationAsync(String organisationName);
        public  Task<IEnumerable<MemberDTO>> ReadMembersAsync(int organisationId);
        public  Task<IEnumerable<EventDTO>> ReadEventsAsync(int organisationId);
        public  Task<IEnumerable<VenueDTO>> ReadVenuesAsync(int eventId);
        #endregion



        #region Updating
        public  Task<Boolean> UpdateMemberAsync(MemberDTO member);
        public  Task<Boolean> UpdateOrganisationAsync(OrganisationDTO organisation);
        public  Task<Boolean> UpdateEventAsync(EventDTO @event);
        public  Task<Boolean> UpdateVenueAsync(VenueDTO venue);
        public  Task<Boolean> UpdateVenueImageAsync(VenueImageDTO image);
        public  Task<Boolean> UpdateAcceptedEmailDomainAsync(AcceptedEmailDomainDTO acceptedEmailDomain);
        public  Task<Boolean> DeleteMemberAsync(MemberDTO member);
        public  Task<Boolean> DeleteOrganisationAsync(OrganisationDTO organisation);
        public  Task<Boolean> DeleteEventAsync(EventDTO @event);
        public  Task<Boolean> DeleteVenueAsync(VenueDTO venue);
        public  Task<Boolean> DeleteVenueImageAsync(VenueImageDTO image);
        public  Task<Boolean> DeleteAcceptedEmailDomainAsync(AcceptedEmailDomainDTO acceptedEmailDomain);
        public  Task<Boolean> DeleteJobAsync(JobDTO job);
        #endregion



        #region Creating
        public  Task<Boolean> CreateMemberAsync(MemberDTO member);
        public  Task<Boolean> CreateOrganisationAsync(OrganisationDTO organisation);
        public  Task<Boolean> CreateEventAsync(EventDTO @event);
        public  Task<Boolean> CreateVenueAsync(VenueDTO venue);
        public  Task<Boolean> CreateVenueImageAsync(VenueImageDTO image);
        public  Task<Boolean> CreateAcceptedEmailDomainAsync(AcceptedEmailDomainDTO acceptedEmailDomain);
        #endregion

        public Task<Boolean> LoginAsync(String userName, String userPassword, String organisationName);
        public Task<Boolean> LogoutAsync();
    }
}
