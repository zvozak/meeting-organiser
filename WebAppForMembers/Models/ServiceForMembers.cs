using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonData;
using CommonData.Entities;
using Microsoft.AspNetCore.Identity;

namespace WebAppForMembers.Models
{
    public class ServiceForMembers : IServiceForMembers
    {
        private readonly MeetingApplicationContext context;
        private readonly UserManager<User> userManager;

        public ServiceForMembers(MeetingApplicationContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public bool SaveChanges()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public EventViewModel ConvertEventToEventViewModel(Event e)
        {
            return new EventViewModel
                {
                    EventId = e.Id,
                    Name = e.Name,
                    DeadlineForApplication = e.DeadlineForApplication,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    OrganisationName = GetOrganisation(e.OrganisationId).Name
                };
        }
        public EventViewModel ConvertEventToEventViewModel(Event e, int userId)
        {
            var result = ConvertEventToEventViewModel(e);

            int memberId = context.Members
               .FirstOrDefault(member =>
               member.OrganisationId == e.OrganisationId &&
               IsMemberOfOrganisation(GetOrganisation(e.OrganisationId).Name, userId))
               .Id;

            var eventForm = GetEventForm(e.Id, memberId);

            result.State = EventViewModel.EventState.Unseen;
            if (eventForm != null)
            {
                result.State = eventForm.WantToJoin ? EventViewModel.EventState.Accepted : EventViewModel.EventState.Rejected;
            }

            return result;
        }

        #region Queries

        public IEnumerable<Organisation> GetOrganisationsOf(int userId)
        {
            var organisationIds =
                from members in context.Members
                join membership in context.Memberships
                on members.Id equals membership.MemberId
                where membership.UserId == userId
                select members.OrganisationId;

            return
            from organisations in context.Organisations
            join ids in organisationIds
                on organisations.Id equals ids
            select organisations;
        }
        public DateTime? StrictestDeadlineAt(int organisationId)
        {
            try
            {
                return
                GetEventsOf(organisationId)
                .OrderBy(info => info.DeadlineForApplication)
                .Select(info => info.DeadlineForApplication)
                .First();
            }
            catch
            {
                return null;
            }
        }
        public Organisation GetOrganisation(int organisationId)
        {
            return
                context.Organisations.FirstOrDefault(
                    organisation => organisation.Id == organisationId);
        }
        public IEnumerable<EventViewModel> GetEventsOf(int organisationId)
        {
            if (context.Events.Count() == 0)
            {
                return new HashSet<EventViewModel>();
            }

            var events = context.Events
                .Where( e => e.OrganisationId == organisationId);

            var eventInfos = new HashSet<EventViewModel>(events.Count());
            foreach(var e in events)
            {
                eventInfos.Add(ConvertEventToEventViewModel(e));
            }

            return eventInfos;
        }

        public IEnumerable<EventViewModel> GetAvailableEventsOf(int userId)
        {
            var events = context.Events
                .Where(e => GetOrganisationsOf(userId)
                .Any( o => o.Id == e.OrganisationId));

            var eventInfos = new HashSet<EventViewModel>(events.Count());
            foreach (var e in events)
            {
                eventInfos.Add(ConvertEventToEventViewModel(e, userId));
            }

            return eventInfos;
        }
        public EventViewModel GetEventViewModel(int eventId)
        {
            return 
                ConvertEventToEventViewModel(
                context.Events.FirstOrDefault(
                    events => events.Id == eventId));
        }
        public Event? GetEvent(int eventId)
        {
            return
                context.Events.FirstOrDefault(
                    events => events.Id == eventId);
        }
        public EventForm? GetEventForm(int eventId, int memberId)
        {
            return
               context.EventForms.FirstOrDefault(eventform =>
                   eventform.EventId == eventId &&
                   eventform.MemberId == memberId);
        }
        public Venue GetVenue(int venueId)
        {
            return context.Venues.FirstOrDefault(venue => venue.Id == venueId);
        }
        public IEnumerable<Venue> GetVenues(int eventId)
        {
            return context.Venues.Where(venue => venue.EventId == eventId);
        }
        public IEnumerable<VenueImage> GetVenueImages(int venueId)
        {
            return context.VenueImages.Where(image => image.VenueId == venueId);
        }
        public Byte[] GetVenueMainImage(int? venueId)
        {
            if (venueId == null)
                return null;

            return context.VenueImages
                .Where(image => image.VenueId == venueId)
                .Select(image => image.ImageSmall)
                .FirstOrDefault();
        }
        public Byte[] GetVenueImage(Int32? imageId, Boolean large)
        {
            if (imageId == null)
                return null;

            Byte[] imageContent = context.VenueImages
                .Where(image => image.Id == imageId)
                .Select(image => large ? image.ImageLarge : image.ImageSmall)
                .FirstOrDefault();

            return imageContent;
        }
        public IEnumerable<int> GetVenueImageIds(int venueId)
        {
            return
                context.VenueImages
                 .Where(image => image.VenueId == venueId)
                 .Select(image => image.Id);
        }

        public bool IsExistingOrganisation(string organisationName)
        {
            return context.Organisations.Any(org => org.Name == organisationName);
        }
        public Organisation GetOrganisationByName(string organisationName)
        {
            return
                context.Organisations.FirstOrDefault(
                 org => org.Name == organisationName);
        }
        public bool IsMemberOfOrganisation(string organisationName, string realUserName, string userEmail)
        {
            Organisation organisation = GetOrganisationByName(organisationName);
            if(organisation == null)
            {
                return false;
            }
            return
                context.Members.Any(member =>
                   member.OrganisationId == organisation.Id &&
                   member.Name == realUserName &&
                   member.Email == userEmail);
        }

        public bool IsMemberOfOrganisation(string organisationName, int userId)
        {
            int organisationId = GetOrganisationByName(organisationName).Id;
            return
                (from members in context.Members
                join memberships in context.Memberships
                on members.Id equals memberships.MemberId
                where members.OrganisationId == organisationId
                select members.Id
                ).Any();
        }

        public int GetNumberOfEventsIn(string organisationName)
        {
            return 
                (from organisation in context.Organisations
                join events in context.Events
                on organisation.Id equals events.OrganisationId
                where organisation.Name == organisationName
                select organisation)
                .Count();
        }
        public Member GetMember(string organisationName, string realUserName, string userEmail)
        {
            Organisation organisation = GetOrganisationByName(organisationName);
            return
                context.Members.FirstOrDefault(member =>
                   member.OrganisationId == organisation.Id &&
                   member.Name == realUserName &&
                   member.Email == userEmail
                    );
        }
        public int GetMemberIdBy(string organisationName, string realUserName, string userEmail)
        {
            Organisation organisation = GetOrganisationByName(organisationName);
            return
                context.Members.FirstOrDefault(member =>
                   member.OrganisationId == organisation.Id &&
                   member.Name == realUserName &&
                   member.Email == userEmail
                    )
                .Id;
        }
        public int GetMemberIdBy(int userId, int organisationId)
        {
            return
                (from member in context.Members
                join memberships in context.Memberships
                on member.Id equals memberships.MemberId
                 where memberships.UserId == userId && member.OrganisationId == organisationId
                select member.Id).FirstOrDefault();
        }
        public User GetUserBy(string userName)
        {
            return context.Users.FirstOrDefault(user => user.UserName == userName);
        }
        public User GetUserBy(int userId)
        {
            return context.Users.FirstOrDefault(user => user.Id == userId);
        }
        public bool IsExistingMembership(int memberId, int userId)
        {
            return context.Memberships.Any(membership => membership.MemberId == memberId && membership.UserId == userId);
        }
        public bool CanJoinOrganisation(string organisationName, string email)
        {
            int organisationId = GetOrganisationByName(organisationName).Id;
            List<AcceptedEmailDomain> acceptedEmailDomains = context.AcceptedEmailDomains
                .Where(domain => domain.OrganisationId == organisationId)
                .ToList();
            string userDomain = email.Substring( email.IndexOf('@')+1);
            return 
                context.Organisations.FirstOrDefault(org => org.Name == organisationName).PermitNewMembers &&
                (acceptedEmailDomains.Count == 0 || 
                acceptedEmailDomains.Any(domain => domain.DomainName.Equals(userDomain)));
        }

        public IEnumerable<Job> PossibleJobsAtOrganisation(int organisationId)
        {
            return context.Jobs
                .Where(job => job.OrganisationId == organisationId);
        }
        public IEnumerable<Project> PossibleProjectsAtOrganisation(int organisationId)
        {
            return context.Projects
                .Where(project => project.OrganisationId == organisationId);
        }
        public Project GetProject(string name, int organisationId)
        {
            return context.Projects.FirstOrDefault(p => 
                    p.Name == name && 
                    p.OrganisationId == organisationId);
        }

        public Job GetJob(string title, int organisationId)
        {
            return context.Jobs.FirstOrDefault(p =>
                    p.Title == title &&
                    p.OrganisationId == organisationId);
        }

        public bool IsEmailAtOrganisation(string OrganisationName, string EmailAddressAtOrganisation)
        {
            int OrganisationId = GetOrganisationByName(OrganisationName).Id;
            return context.Members.Any(member =>
            member.OrganisationId == OrganisationId &&
            member.Email == EmailAddressAtOrganisation);
        }


        #endregion

        #region Create

        public bool CreateOrganisation(Organisation organisation)
        {
            context.Organisations.Add(organisation);

            return SaveChanges();
        }

        public bool CreateMembership(string organisationName, int userId, string userEmail)
        {
            string realUserName = GetUserBy(userId).Name;
            context.Memberships.Add(
                new Membership()
                {
                    MemberId = GetMemberIdBy(organisationName, realUserName, userEmail),
                    UserId = userId
                });
            
            return SaveChanges();
        }

        public bool CreateMemberships(string userName)
        {
            User user = GetUserBy(userName);
            HashSet<int> memberIds = context.Members.Where(m => m.Email == user.Email && m.Name == user.Name).Select(m => m.Id).ToHashSet();

            foreach (var id in memberIds)
            {
                context.Memberships.Add( new Membership
                    {
                        MemberId = id,
                        UserId = user.Id
                    });
            }

            return SaveChanges();
        }

        public bool AddMember(Member member)
        {
            context.Members.Add(member);
            
            return SaveChanges();
        }

        public bool CreateJob( string jobTitle, int organisationId)
        {
            bool isExistingOrganisationId = context.Organisations.Any(o => o.Id == organisationId);
            if (!isExistingOrganisationId)
            {
                return false;
            }

            if(context.Jobs.Any(j => j.Title == jobTitle && j.OrganisationId == organisationId))
            {
                return true;
            }

            context.Jobs.Add(new Job
            {
                OrganisationId = organisationId,
                Title = jobTitle
            });
            
            return SaveChanges();
        }

        public bool CreateProject(string projectName, int organisationId)
        {
            context.Projects.Add(new Project
            {
                Name = projectName,
                OrganisationId = organisationId
            });
            
            return SaveChanges();
        }

        public bool CreateMemberOfProject(int memberId, int projectId)
        {
            context.MemberOfProjects.Add(new MemberOfProject { 
                MemberId = memberId,
                ProjectId = projectId
            });
            
            return SaveChanges();
        }

        public bool CreateEventForm(EventForm eventForm)
        {
            context.EventForms.Add(eventForm);
            return SaveChanges();
        }

        #endregion


        #region Edit

        public bool RejectEvent(int memberId, int eventId)
        {
            var eventForm = context.EventForms.FirstOrDefault(e => e.MemberId == memberId && e.EventId == eventId);
            
            if(eventForm == null)
            {
                return false;
            }

            eventForm.WantToJoin = false;
            
            return SaveChanges();
        }
        public bool AcceptEvent(int memberId, int eventId)
        {
            var eventForm = context.EventForms.FirstOrDefault(e => e.MemberId == memberId && e.EventId == eventId);

            if (eventForm == null)
            {
                return false;
            }

            eventForm.WantToJoin = true;

            return SaveChanges();
        }
        public bool EditEventForm(EventForm eventForm)
        {
            var eventFormToChange = context.EventForms.FirstOrDefault(e => e.MemberId == eventForm.MemberId && e.EventId == eventForm.EventId);

            if (eventFormToChange == null)
            {
                return false;
            }

            eventFormToChange = eventForm;

            return SaveChanges();
        }

        #endregion



        #region Deleting

        public bool DeleteMembership(int userId, int organisationId)
        {
            bool isValidUser = context.Users.Any(user => user.Id == userId);
            bool isValidOrganisation = context.Organisations.Any(o => o.Id == organisationId);
            if (!isValidUser || !isValidOrganisation)
            {
                return false;
            }

            var memberIdsOfOrganisation = context.Members
                    .Where(member => member.OrganisationId == organisationId)
                    .Select(member => member.Id);
            
            var membership =
                    context.Memberships.FirstOrDefault( membership =>
                    membership.UserId == userId &&
                    memberIdsOfOrganisation.Contains(membership.MemberId));

            if (membership == null)
            {
                return false;
            }
            
            context.Memberships.Remove( membership);

            return SaveChanges();
        }

        #endregion
    }
}
