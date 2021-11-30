using CommonData.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingOrganiserDesktopApp.Model
{
    public interface IMeetingApplicationModel
    {
        public List<MemberDTO> GuestList
        {
            get;
        }

        public OrganisationDTO Organisation
        {
            get;
        }

        public IReadOnlyList<MemberDTO> Members
        {
            get;
        }


        public IReadOnlyList<EventDTO> Events
        {
            get;
        }


        public Boolean IsUserLoggedIn { get; }


        public event EventHandler<MemberEventArgs> MemberChanged;
        public event EventHandler<JobEventArgs> JobChanged;
        public event EventHandler<EventEventArgs> EventChanged;
        public event EventHandler<VenueEventArgs> VenueChanged;
        public event EventHandler<EventEventArgs> GuestListCreated;


        public bool IsExistingJobTitle(string jobtitle);
        public bool IsExistingBoss(string name);

        #region Create

        public void CreateMember(MemberDTO member);

        public void CreateJob(JobDTO job);


        public void CreateEvent(EventDTO @event);


        public void CreateVenue(VenueDTO venue);


        public void CreateImage(Int32 eventId, Int32 venueId, Byte[] imageSmall, Byte[] imageLarge);
        #endregion


        #region Update
        public void UpdateEvent(EventDTO @event);

        public void UpdateVenue(VenueDTO venue);


        public void UpdateMember(MemberDTO member);

        public void UpdateJob(JobDTO job);


        public void UpdateOrganisation(OrganisationDTO newOrganisation);
        #endregion


        #region Delete

        public void DeleteEvent(EventDTO @event);
        public void DeleteVenue(VenueDTO venue);
        public void DeleteMember(MemberDTO member);
        public void DeleteJob(JobDTO job);

        public void DeleteImage(VenueImageDTO image);


        #endregion


        public  Task LoadAsync(String organisationName);
        public Task LoadAsync ();

        public  Task SaveAsync();


        public  Task<Boolean> LoginAsync(String userName, String userPassword, String organisationName);


        public  Task<Boolean> LogoutAsync();


        public void CreateGuestList(Int32 eventId);

    }
}