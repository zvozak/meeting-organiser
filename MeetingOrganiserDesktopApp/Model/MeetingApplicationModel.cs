using MeetingOrganiserDesktopApp.Persistence;
using CommonData.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingOrganiserDesktopApp.Model
{

    public class MeetingApplicationModel : IMeetingApplicationModel
    {

        private enum DataFlag
        {
            Create,
            Update,
            Delete
        }

        private IMeetingApplicationPersistence persistence;
        private OrganisationDTO organisation;
        private List<MemberDTO> members;
        private List<EventDTO> events;
        private Dictionary<MemberDTO, DataFlag> memberFlags;
        private Dictionary<JobDTO, DataFlag> jobFlags;
        private Dictionary<EventDTO, DataFlag> eventFlags;
        private Dictionary<VenueDTO, DataFlag> venueFlags;
        private Dictionary<VenueImageDTO, DataFlag> imageFlags;

        public List<MemberDTO> GuestList
        {
            get; private set;
        }

        public MeetingApplicationModel(IMeetingApplicationPersistence persistence)
        {
            if (persistence == null)
                throw new ArgumentNullException(nameof(persistence));

            IsUserLoggedIn = false;
            this.persistence = persistence;
        }

        public OrganisationDTO Organisation
        {
            get { return organisation; }
            private set
            {
                if (organisation != value)
                {
                    organisation = value;
                }
            }
        }

        public bool IsHierarchical
        {
            get { return Organisation.TypeOfStructure == CommonData.Entities.TypeOfStructure.Hierarchical; }
        }

        public bool IsProjectBased
        {
            get { return Organisation.TypeOfStructure == CommonData.Entities.TypeOfStructure.ProjectBased; }
        }

        public IReadOnlyList<MemberDTO> Members
        {
            get { return members; }
        }


        public IReadOnlyList<EventDTO> Events
        {
            get { return events; }
        }


        public Boolean IsUserLoggedIn { get; private set; }


        public event EventHandler<MemberEventArgs> MemberChanged;
        public event EventHandler<JobEventArgs> JobChanged;
        public event EventHandler<EventEventArgs> EventChanged;
        public event EventHandler<VenueEventArgs> VenueChanged;
        public event EventHandler<EventEventArgs> GuestListCreated;


        public bool IsExistingJobTitle (string jobtitle)
        {
            return Organisation.Jobs.Any(j => j.Title == jobtitle);
        }
        public bool IsExistingBoss(string name)
        {
            return members.Any(m => m.Name == name);
        }

        #region Create

        public void CreateMember(MemberDTO member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (members.Contains(member))
                throw new ArgumentException("The member is already in the collection.", nameof(member));
            if (!organisation.Jobs.Any( j => j.Title == member.Job.Title))
                throw new ArgumentException("The jobtitle given to member does not exist.", nameof(member));

            if (member.JobId == null)
            {
                member.JobId = organisation.Jobs.FirstOrDefault(j => j.Title == member.Job.Title).Id;
            }
            if (member.BossId == null)
            {
                member.BossId = members.FirstOrDefault(b => b.Name == member.Boss.Name).Id;
            }

            member.Id = ((members.Count > 0 ? members.Max(b => b.Id) : 0) + 1); // temporary Id
            memberFlags.Add(member, DataFlag.Create);
            members.Add(member);
        }


        public void CreateJob(JobDTO job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            if (organisation.Jobs.Contains(job))
                throw new ArgumentException("The job is already in the collection.", nameof(job));

            if (organisation.Jobs.Count > 0)
            {
                job.Id = organisation.Jobs.Max(j => j.Id) + 1; // temporary Id
            }
            else
            {
                job.Id = 1; // temporary Id
            }

            jobFlags.Add(job, DataFlag.Create);

            OnJobChanged(jobId: job.Id);
        }


        public void CreateEvent(EventDTO @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (events.Contains(@event))
                throw new ArgumentException("The event is already in the collection.", nameof(@event));

            @event.Id = (events.Count > 0 ? events.Max(b => b.Id) : 0) + 1; // temporary Id
            eventFlags.Add(@event, DataFlag.Create);
            events.Add(@event);
        }


        public void CreateVenue(VenueDTO venue)
        {
            if (venue == null)
                throw new ArgumentNullException(nameof(venue));
            if (events.Any (e => e.Venues.Contains (venue)))
                throw new ArgumentException("The venue is already in the collection.", nameof(venue));

            if (events.Count > 0)
            {
                venue.Id = events.Max(e => e.Venues.Count() > 0 ? e.Venues.Max(v => v.Id) : 0) + 1; // temporary Id
            }
            else
            {
                venue.Id = 1; // temporary Id
            }

            
            Int32 index = events.IndexOf(events.FirstOrDefault(e => e.Id == venue.EventId));
            EventDTO @event = events[index];
            @event.Venues.Add(venue);
            events.RemoveAt(index);
            events.Insert(index, @event);
            venueFlags.Add(venue, DataFlag.Create);

            OnVenueChanged(eventId: @event.Id, venueId: venue.Id);
        }


        public void CreateImage(Int32 eventId, Int32 venueId, Byte[] imageSmall, Byte[] imageLarge)
        {
            EventDTO eventDTO = events
                .FirstOrDefault(e => e.Id == eventId);
            if (eventDTO == null)
                throw new ArgumentException("No event exists with this venue.", nameof(venueId));

            VenueDTO venue = eventDTO.Venues
                .FirstOrDefault (v => v.Id == venueId);
            if (venue == null)
                throw new ArgumentException("The venue does not exist.", nameof(venueId));

            int id = events.Max(e => e.Venues.Any() ? e.Venues.Max(v => v.Images.Any() ? v.Images.Max(im => im.Id) : 0) : 0) + 1;
            VenueImageDTO image = new VenueImageDTO 
            { 
                Id = id,
                VenueId = venueId, 
                ImageSmall = imageSmall, 
                ImageLarge = imageLarge 
            };

            venue.Images.Add(image);
            imageFlags.Add(image, DataFlag.Create);

            OnVenueChanged(eventId: eventDTO.Id, venueId: venueId);
        }
        #endregion




        #region Update
        public void UpdateEvent(EventDTO @event)
        { 
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            EventDTO eventToModify = events.FirstOrDefault(b => b.Id == @event.Id);

            if (eventToModify == null)
                throw new ArgumentException("The event does not exist.", nameof(@event));

            eventToModify.StartDate                 = @event.StartDate;
            eventToModify.EndDate                   = @event.EndDate;
            eventToModify.DeadlineForApplication    = @event.DeadlineForApplication;
            eventToModify.Description               = @event.Description;
            eventToModify.GuestLimit                = @event.GuestLimit;
            eventToModify.Name                      = @event.Name;
            eventToModify.IsConnectedGraphRequired  = @event.IsConnectedGraphRequired;
            eventToModify.IsWeightRequired          = @event.IsWeightRequired;
            eventToModify.ProjectImportanceWeight   = @event.ProjectImportanceWeight;
            eventToModify.NumberOfProjectsWeight    = @event.NumberOfProjectsWeight;
            eventToModify.NumberOfSubordinatesWeight = @event.NumberOfSubordinatesWeight;
            eventToModify.NumberOfNeighboursWeight  = @event.NumberOfNeighboursWeight;
            eventToModify.JobWeight                 = @event.JobWeight;
            eventToModify.Venues                    = @event.Venues;

            if (eventFlags.ContainsKey(eventToModify) && eventFlags[eventToModify] == DataFlag.Create)
            {
                eventFlags[eventToModify] = DataFlag.Create;
            }
            else
            {
                eventFlags[eventToModify] = DataFlag.Update;
            }

            OnEventChanged(@event.Id);
        }


        public void UpdateMember(MemberDTO member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            MemberDTO memberToModify = members.FirstOrDefault(b => b.Id == member.Id);

            if (memberToModify == null)
                throw new ArgumentException("The member does not exist.", nameof(member));

            if ( !organisation.Jobs.Any(j => j.Title == member.Job.Title))
            {
                throw new ArgumentException("The role/job title provided for member does not exist.", nameof(member.Job));
            }

            bool jobChanged = !organisation.Jobs.Any(j => j.Id == member.Job.Id && j.Title == member.Job.Title);
            if (jobChanged)
            {
                memberToModify.Job = organisation.Jobs.Single(j => j.Title == member.Job.Title);
                memberToModify.JobId = memberToModify.Job.Id;
            }
            bool bossChanged = !members.Any(m => m.Id == member.Boss.Id && m.Name == member.Boss.Name);
            if (bossChanged)
            {
                memberToModify.Boss = members.Single(m => m.Name == member.Boss.Name);
                memberToModify.BossId = memberToModify.Boss.Id;
            }
            memberToModify.Name = member.Name;
            memberToModify.Department = member.Department;
            memberToModify.Email = member.Email;
            memberToModify.DateOfJoining = member.DateOfJoining;

            if (memberFlags.ContainsKey(memberToModify) && memberFlags[memberToModify] == DataFlag.Create)
            {
                memberFlags[memberToModify] = DataFlag.Create;
            }
            else
            {
                memberFlags[memberToModify] = DataFlag.Update;
            }

            OnMemberChanged(member.Id);
        }

        public void UpdateJob(JobDTO job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            var eventdto = events.FirstOrDefault(e => e.Id == job.Id);
            if (eventdto == null)
                throw new ArgumentException("No event exists for this job.", nameof(job));

            JobDTO jobToModify = organisation.Jobs.FirstOrDefault(b => b.Id == job.Id);

            if (jobToModify == null)
                throw new ArgumentException("The member does not exist.", nameof(job));

            jobToModify.Title = job.Title;
            jobToModify.Weight = job.Weight;

            if (jobFlags.ContainsKey(jobToModify) && jobFlags[jobToModify] == DataFlag.Create)
            {
                jobFlags[jobToModify] = DataFlag.Create;
            }
            else
            {
                jobFlags[jobToModify] = DataFlag.Update;
            }

            var membersWithModifiedJob = members.FindAll(m => m.Job.Id == job.Id);
            foreach (var member in membersWithModifiedJob)
            {
                member.Job = job;
                OnMemberChanged(member.Id);
            }

            OnJobChanged(jobId: job.Id);
        }

        public void UpdateVenue(VenueDTO venue)
        {
            if (venue == null)
                throw new ArgumentNullException(nameof(venue));

            var eventdto = events.FirstOrDefault(e => e.Id == venue.EventId);
            if (eventdto == null)
                throw new ArgumentException("No event exists for this venue.", nameof(venue));

            VenueDTO venueToModify = eventdto.Venues.FirstOrDefault(b => b.Id == venue.Id);

            if (venueToModify == null)
                throw new ArgumentException("The member does not exist.", nameof(venue));

            venueToModify.Name = venue.Name;
            venueToModify.LocationX = venue.LocationX;
            venueToModify.LocationY = venue.LocationY;
            venueToModify.GuestLimit = venue.GuestLimit;
            venueToModify.Address = venue.Address;
            venueToModify.Description = venue.Description;
            venueToModify.Images = venue.Images;

            if (venueFlags.ContainsKey(venueToModify) && venueFlags[venueToModify] == DataFlag.Create)
            {
                venueFlags[venueToModify] = DataFlag.Create;
            }
            else
            {
                venueFlags[venueToModify] = DataFlag.Update;
            }

            OnVenueChanged(eventId: eventdto.Id, venueId: venue.Id);
        }


        public void UpdateOrganisation(OrganisationDTO newOrganisation)
        {
            if (newOrganisation == null)
                throw new ArgumentNullException(nameof(newOrganisation));
            
            organisation = newOrganisation;
            
            // TODO OnOrganisationChanged(newOrganisation.Id);
        }
        #endregion

        #region Delete

        public void DeleteMember(MemberDTO member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            MemberDTO memberToDelete = members.FirstOrDefault(b => b.Id == member.Id);

            if (memberToDelete == null)
                throw new ArgumentException("The member does not exist.", nameof(member));

            if (memberFlags.ContainsKey(memberToDelete) && memberFlags[memberToDelete] == DataFlag.Create)
                memberFlags.Remove(memberToDelete);
            else
                memberFlags[memberToDelete] = DataFlag.Delete;

            members.Remove(memberToDelete);
        }

        public void DeleteJob(JobDTO job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            JobDTO jobToDelete = organisation.Jobs.FirstOrDefault(b => b.Id == job.Id);

            if (jobToDelete == null)
                throw new ArgumentException("The job does not exist.", nameof(job));

            if (jobFlags.ContainsKey(jobToDelete) && jobFlags[jobToDelete] == DataFlag.Create)
                jobFlags.Remove(jobToDelete);
            else
                jobFlags[jobToDelete] = DataFlag.Delete;

            organisation.Jobs.Remove(jobToDelete);
        }

        public void DeleteEvent(EventDTO @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            EventDTO eventToDelete = events.FirstOrDefault(b => b.Id == @event.Id);

            if (eventToDelete == null)
                throw new ArgumentException("The event does not exist.", nameof(@event));

            if (eventFlags.ContainsKey(eventToDelete) && eventFlags[eventToDelete] == DataFlag.Create)
                eventFlags.Remove(eventToDelete);
            else
                eventFlags[eventToDelete] = DataFlag.Delete;

            events.Remove(eventToDelete);
        }

        public void DeleteVenue(VenueDTO venue)
        {
            if (venue == null)
                throw new ArgumentNullException(nameof(venue));

            EventDTO eventDto = events.FirstOrDefault(b => b.Id == venue.EventId);
            if (eventDto == null)
                throw new ArgumentException("No event exist with this venue.", nameof(venue));

            VenueDTO venueToDelete = eventDto.Venues.FirstOrDefault(b => b.Id == venue.Id);

            if (venueToDelete == null)
                throw new ArgumentException("The venue does not exist.", nameof(venue));

            if (venueFlags.ContainsKey(venueToDelete) && venueFlags[venueToDelete] == DataFlag.Create)
                venueFlags.Remove(venueToDelete);
            else
                venueFlags[venueToDelete] = DataFlag.Delete;

            eventDto.Venues.Remove(venueToDelete);
        }

        public void DeleteImage(VenueImageDTO image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            EventDTO eventDto = events.FirstOrDefault(e => e.Venues.Any(v => v.Id == image.VenueId));
            if (eventDto == null)
                throw new ArgumentException("No event exists with the venue ID of this image.", nameof(image));

            VenueDTO venue = eventDto.Venues.FirstOrDefault(v => v.Id == image.VenueId);
            if (venue == null)
                throw new ArgumentException("No venue exists with this image.", nameof(image));

            VenueImageDTO imageToDelete = venue.Images.FirstOrDefault(b => b.Id == image.Id);
            if (imageToDelete == null)
                throw new ArgumentException("The image does not exist.", nameof(image));

            if (imageFlags.ContainsKey(imageToDelete) && imageFlags[imageToDelete] == DataFlag.Create)
                imageFlags.Remove(imageToDelete);
            else
                imageFlags[imageToDelete] = DataFlag.Delete;

            venue.Images.Remove(imageToDelete);
        }
        #endregion

        public async Task LoadAsync()
        {
            await LoadAsync(organisation.Name);
        }
        public async Task LoadAsync(String organisationName)
        {
            organisation = persistence.ReadOrganisationAsync (organisationName);
            members      = (await persistence.ReadMembersAsync (organisation.Id)).ToList ();
            events       = (await persistence.ReadEventsAsync (organisation.Id)).ToList ();

            memberFlags = new Dictionary <MemberDTO, DataFlag> ();
            eventFlags  = new Dictionary <EventDTO, DataFlag> ();
            venueFlags = new Dictionary<VenueDTO, DataFlag>();
            imageFlags  = new Dictionary <VenueImageDTO, DataFlag> ();
        }

        private async Task SaveEventsAsync()
        {
            List<EventDTO> eventsToSave = eventFlags.Keys.ToList();

            foreach (EventDTO eventDTO in eventsToSave)
            {
                Boolean result = true;

                switch (eventFlags[eventDTO])
                {
                    case DataFlag.Create:
                        result = await persistence.CreateEventAsync(eventDTO);
                        break;
                    case DataFlag.Delete:
                        result = await persistence.DeleteEventAsync(eventDTO);
                        break;
                    case DataFlag.Update:
                        result = await persistence.UpdateEventAsync(eventDTO);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + eventFlags[eventDTO] + " failed on event " + eventDTO.Id);

                eventFlags.Remove(eventDTO);
            }
        }

        private async Task SaveVenuesAsync()
        {
            List<VenueDTO> venuesToSave = venueFlags.Keys.ToList();

            foreach (VenueDTO venueDTO in venuesToSave)
            {
                Boolean result = true;

                switch (venueFlags[venueDTO])
                {
                    case DataFlag.Create:
                        result = await persistence.CreateVenueAsync(venueDTO);
                        break;
                    case DataFlag.Delete:
                        result = await persistence.DeleteVenueAsync(venueDTO);
                        break;
                    case DataFlag.Update:
                        result = await persistence.UpdateVenueAsync(venueDTO);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + venueFlags[venueDTO] + " failed on venue " + venueDTO.Id);

                venueFlags.Remove(venueDTO);
            }
        }

        private async Task SaveImagesAsync()
        {
            List<VenueImageDTO> imagesToSave = imageFlags.Keys.ToList();

            foreach (VenueImageDTO image in imagesToSave)
            {
                Boolean result = true;

                switch (imageFlags[image])
                {
                    case DataFlag.Create:
                        result = await persistence.CreateVenueImageAsync(image);
                        break;
                    case DataFlag.Delete:
                        result = await persistence.DeleteVenueImageAsync(image);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + imageFlags[image] + " failed on image " + image.Id);

                imageFlags.Remove(image);
            }
        }

        private async Task SaveMembersAsync()
        {
            List<MemberDTO> membersToSave = memberFlags.Keys.ToList();

            foreach (MemberDTO memberDTO in membersToSave)
            {
                Boolean result = true;

                switch (memberFlags[memberDTO])
                {
                    case DataFlag.Create:
                        result = await persistence.CreateMemberAsync(memberDTO);
                        break;
                    case DataFlag.Delete:
                        result = await persistence.DeleteMemberAsync(memberDTO);
                        break;
                    case DataFlag.Update:
                        result = await persistence.UpdateMemberAsync(memberDTO);
                        break;
                }

                if (!result)
                    throw new InvalidOperationException("Operation " + memberFlags[memberDTO] + " failed on member " + memberDTO.Id);

                memberFlags.Remove(memberDTO);
            }
        }
        public async Task SaveAsync()
        {
            await SaveEventsAsync();
            await SaveVenuesAsync();
            await SaveImagesAsync();
            await SaveMembersAsync();
        }


        public async Task<Boolean> LoginAsync(String userName, String userPassword, String organisationName)
        {
            IsUserLoggedIn = await persistence.LoginAsync(userName, userPassword, organisationName);
            return IsUserLoggedIn;
        }


        public async Task<Boolean> LogoutAsync()
        {
            if (!IsUserLoggedIn)
                return true;

            IsUserLoggedIn = !(await persistence.LogoutAsync());

            return IsUserLoggedIn;
        }

        private void OnGuestListCreated(Int32 eventId)
        {
            if (GuestListCreated != null)
            {
                GuestListCreated(this, new EventEventArgs { EventId = eventId });
            }
        }

        private void OnMemberChanged(Int32 memberId)
        {
            if (MemberChanged != null)
                MemberChanged(this, new MemberEventArgs { MemberId = memberId });
        }
        private void OnJobChanged(Int32 jobId)
        {
            if (JobChanged != null)
                JobChanged(this, new JobEventArgs { JobId = jobId });
        }
        private void OnEventChanged(Int32 eventId)
        {
            if (EventChanged != null)
                EventChanged(this, new EventEventArgs { EventId = eventId });
        }
        private void OnVenueChanged(Int32 venueId, Int32 eventId)
        {
            if (VenueChanged != null)
                VenueChanged(this, new VenueEventArgs { EventId = eventId, VenueId = venueId });
        }


        private void ConstructEdgesFromHierarchicalStructure<TNode>(ref GenericGraph<TNode> graph)
            where TNode : Node
        {
            foreach (var member in members)
            {
                if (member.BossId != null && member.BossId > 0)
                {
                    graph.AddEdge( (int)member.BossId, member.Id);
                }
            }
        }


        private void ConstructEdgesFromProjectBasedStructure<TNode>(ref GenericGraph<TNode> graph)
            where TNode : Node
        {
            foreach (var project in organisation.Projects)
            {
                var projectMembers = members.Where(m => m.Projects.Contains(project)).ToList();
                foreach (var member in projectMembers)
                {
                    foreach (var neighbour in projectMembers)
                    {
                        if (member != neighbour)
                        {
                            graph.AddEdge(member.Id, neighbour.Id);
                        }
                    }
                }
            }
        }

        
        private GenericGraph<TNode> ConstructGenericGraph<TNode>(Func<int, TNode> constructor)
            where TNode : Node
        {
            GenericGraph<TNode> graph = new GenericGraph<TNode>(members.Count);

            foreach (MemberDTO member in members)
            {
                graph.AddNode(member.Id, (id) => constructor(id));
            }

            if (IsHierarchical)
            {
                ConstructEdgesFromHierarchicalStructure (ref graph);
            }
            else
            {
                ConstructEdgesFromProjectBasedStructure (ref graph);
            }

            return graph;
        }

        private int CalculateWeightInHierarchical(int numberOfNeighbours, EventDTO eventDTO, MemberDTO member)
        {
            if (eventDTO.NumberOfSubordinatesWeight == 0)
            {
                return 0;
            }
            int weight = eventDTO.NumberOfSubordinatesWeight * numberOfNeighbours;
            if (member.Boss != null)
            {
                weight--;
            }
            return weight;
        }
        private int CalculateWeightInProjectBased( EventDTO eventDTO, MemberDTO member)
        {
            if (eventDTO.ProjectImportanceWeight == 0)
            {
                return 0;
            }

            int weight = 0;

            foreach (var project in member.Projects)
            {
                weight += eventDTO.ProjectImportanceWeight * project.Weight;
            }

            weight += eventDTO.NumberOfProjectsWeight * member.Projects.Count;

            return weight;
        }
        private int CalculateWeight(int numberOfNeighbours, EventDTO eventDTO, int memberId)
        {
            int weight;
            MemberDTO member = members.Single(m => m.Id == memberId);

            if (IsHierarchical)
            {
                weight = CalculateWeightInHierarchical(numberOfNeighbours, eventDTO, member) + CalculateWeightInProjectBased(eventDTO, member);
            }
            else
            {
                weight = CalculateWeightInProjectBased(eventDTO, member);
            }

            if (eventDTO.JobWeight > 0)
            {
                weight += eventDTO.JobWeight * members.Find(m => m.Id == memberId).Job.Weight;
            }
            weight += eventDTO.NumberOfNeighboursWeight * numberOfNeighbours;
            
            return weight;
        }


        private NodeWeightedGraph ConstructWeightedGraph(EventDTO eventDTO)
        {
            NodeWeightedGraph graph = (NodeWeightedGraph) ConstructGenericGraph((id) => new WeightedNode(id));

            foreach (var member in members)
            {
                int numberOfNeighbours = graph.GetNumberOfNeighbours(member.Id);
                CalculateWeight(numberOfNeighbours, eventDTO, member.Id);
            }

            return graph;
        }

        private Graph ConstructGraph()
        {
            var generic = ConstructGenericGraph<Node>((id) => new Node(id));
            HashSet<Node> nodes = new HashSet<Node>(generic.NumberOfNodes);
            
            Graph graph = new Graph(10);
            foreach (var node in generic.Nodes)
            {
                graph.AddNode((Node)node);
            }
            return graph;
        }


        public List<MemberDTO> CreateGuestListWithWeights(EventDTO eventDTO)
        {
            NodeWeightedGraph graph = ConstructWeightedGraph(eventDTO);
            HashSet<int> memberIds = new HashSet<int>(eventDTO.GuestLimit);

            

            if (eventDTO.IsConnectedGraphRequired)
            {
                memberIds = graph.ConstructCDS_WithCDOM();
            }
            else
            {
                memberIds = graph.ConstructDominatingSet();
            }

            return members.Join(
                    memberIds,
                    member => member.Id,
                    id => id,
                    (member, id) => member)
                .ToList();
        }


        public List<MemberDTO> CreateGuestListWithoutWeights(EventDTO eventDTO)
        {
            Graph graph = ConstructGraph();
            HashSet<int> memberIds = new HashSet<int>(eventDTO.GuestLimit);

            if (eventDTO.IsConnectedGraphRequired)
            {
                memberIds = graph.ConstructCDS_WithCDOM();
            }
            else
            {
                memberIds = graph.ConstructDominatingSet();
            }

            return members.Join(
                    memberIds, 
                    member => member.Id, 
                    id => id, 
                    (member, id) => member)
                .ToList();
        }

        public List<MemberDTO> CreateGuestList_CDSFromHierarchical(EventDTO eventDTO)
        {
            Graph graph = ConstructGraph();
            HashSet<int> memberIds = new HashSet<int>(eventDTO.GuestLimit);

            if (organisation.TypeOfStructure == CommonData.Entities.TypeOfStructure.Hierarchical &&
                eventDTO.IsConnectedGraphRequired)
            {
                memberIds = graph.ConstructCDSInForest(members.First().Id);
            }

            return members.Join(
                    memberIds,
                    member => member.Id,
                    id => id,
                    (member, id) => member)
                .ToList();
        }

        public void CreateGuestList(Int32 eventId)
        {
            EventDTO eventDTO = events.Single(e => e.Id == eventId);

            if (organisation.TypeOfStructure == CommonData.Entities.TypeOfStructure.Hierarchical &&
                eventDTO.IsConnectedGraphRequired)
            {
                GuestList = CreateGuestList_CDSFromHierarchical(eventDTO);
            } 
            else if (eventDTO.IsWeightRequired)
            {
                GuestList = CreateGuestListWithWeights(eventDTO);
            }
            else
            {
                GuestList = CreateGuestListWithoutWeights(eventDTO);
            }

            OnGuestListCreated(eventId);
        }
    }
}
