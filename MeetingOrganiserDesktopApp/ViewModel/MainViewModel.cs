using CommonData.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingOrganiserDesktopApp.Model;
using MeetingOrganiserDesktopApp.Persistence;

namespace MeetingOrganiserDesktopApp.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private IMeetingApplicationModel model;
        private ObservableCollection<EventDTO> events;
        private ObservableCollection<MemberDTO> members;
        private EventDTO selectedEvent;
        private VenueDTO selectedVenue;
        private MemberDTO selectedMember;
        private Boolean isLoaded;

        public DateTime DefaultDateTime => DateTime.Now;
        public DateTime MinimumDateTime => new DateTime(1980, 1, 1, 0, 0, 0);

        public List<MemberDTO> GuestList { get; private set; }

        public bool IsValidEvent
        {
            get
            {
                return EditedEvent.StartDate <= EditedEvent.DeadlineForApplication &&
                       EditedEvent.DeadlineForApplication <= EditedEvent.EndDate;
            }
        }
        public bool IsInvalidEvent
        {
            get { return !IsValidEvent; }
        }

        public ObservableCollection<EventDTO> Events
        {
            get { return events; }
            private set
            {
                if (events != value)
                {
                    events = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<MemberDTO> Members
        {
            get { return members; }
            private set
            {
                if (members != value)
                {
                    members = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsLoaded
        {
            get { return isLoaded; }
            private set
            {
                if (isLoaded != value)
                {
                    isLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        public EventDTO SelectedEvent
        {
            get { return selectedEvent; }
            set
            {
                if (selectedEvent != value)
                {
                    selectedEvent = value;
                    OnPropertyChanged();
                }
            }
        }

        public VenueDTO SelectedVenue
        {
            get { return selectedVenue; }
            set
            {
                if (selectedVenue != value)
                {
                    selectedVenue = value;
                    OnPropertyChanged();
                }
            }
        }

        public MemberDTO SelectedMember
        {
            get { return selectedMember; }
            set
            {
                if (selectedMember != value)
                {
                    selectedMember = value;
                    OnPropertyChanged();
                }
            }
        }

        public EventDTO EditedEvent { get; private set; }


        public DelegateCommand CreateEventCommand { get; private set; }

        public DelegateCommand CreateVenueCommand { get; private set; }

        public DelegateCommand UpdateVenueCommand { get; private set; }

        public DelegateCommand DeleteVenueCommand { get; private set; }

        public DelegateCommand CreateImageCommand { get; private set; }

        public DelegateCommand UpdateEventCommand { get; private set; }

        public DelegateCommand DeleteEventCommand { get; private set; }

        public DelegateCommand CreateGuestListCommand { get; private set; }
        
        public DelegateCommand DeleteImageCommand { get; private set; }

        public DelegateCommand SaveEventChangesCommand { get; private set; }
        public DelegateCommand SaveVenueChangesCommand { get; private set; }

        public DelegateCommand CancelEventChangesCommand { get; private set; }
        public DelegateCommand CancelVenueChangesCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand LoadCommand { get; private set; }

        public DelegateCommand SaveCommand { get; private set; }
        public VenueDTO EditedVenue { get; private set; }

        public event EventHandler<EventEventArgs> GuestListQuery;

        public event EventHandler<EventEventArgs> GuestListCreated;

        public event EventHandler ExitApplication;

        public event EventHandler EventEditingStarted;

        public event EventHandler EventEditingFinished;

        public event EventHandler VenueEditingStarted;

        public event EventHandler VenueEditingFinished;

        public event EventHandler<VenueEventArgs> ImageEditingStarted;

        public MainViewModel(IMeetingApplicationModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            this.model = model;
            this.model.EventChanged += Model_EventChanged;
            this.model.VenueChanged += Model_VenueChanged;
            this.model.GuestListCreated += Model_GuestListCreated;
            isLoaded = false;

            CreateGuestListCommand = new DelegateCommand(param => OnGuestListQuery((param as EventDTO).Id));
            CreateEventCommand = new DelegateCommand(param =>
            {
                EditedEvent = new EventDTO();

                EditedEvent.OrganisationId = model.Organisation.Id;
                EditedEvent.StartDate = DefaultDateTime;
                EditedEvent.DeadlineForApplication = DefaultDateTime;
                EditedEvent.EndDate = DefaultDateTime;

                OnEventEditingStarted();
            });
            UpdateEventCommand = new DelegateCommand(param => UpdateEvent(param as EventDTO));
            DeleteEventCommand = new DelegateCommand(param => DeleteEvent(param as EventDTO));

            CreateVenueCommand = new DelegateCommand(param =>
            {
                EditedVenue = new VenueDTO();
                EditedVenue.EventId = (int)param;
                OnVenueEditingStarted();
            });
            UpdateVenueCommand = new DelegateCommand(param => UpdateVenue(param as VenueDTO));
            DeleteVenueCommand = new DelegateCommand(param => DeleteVenue(param as VenueDTO));

            CreateImageCommand = new DelegateCommand(param =>
            {
                VenueDTO venue = param as VenueDTO;
                OnImageEditingStarted(venue.EventId, venue.Id);
            });
            
            DeleteImageCommand = new DelegateCommand(param => DeleteImage(param as VenueImageDTO));

            SaveEventChangesCommand = new DelegateCommand(param => SaveEventChanges());
            SaveVenueChangesCommand = new DelegateCommand(param => SaveVenueChanges());
            CancelEventChangesCommand = new DelegateCommand(param => CancelEventChanges());
            CancelVenueChangesCommand = new DelegateCommand(param => CancelVenueChanges());
            LoadCommand = new DelegateCommand(param => LoadAsync(param as String));
            SaveCommand = new DelegateCommand(param => SaveAsync());
            ExitCommand = new DelegateCommand(param => OnExitApplication());
        }

        private void Model_GuestListCreated(object sender, EventEventArgs e)
        {
            GuestList = model.GuestList;
            OnGuestListCreated(e.EventId);
        }

        private void OnGuestListQuery(Int32 eventId)
        {
            if (GuestListQuery != null)
                GuestListQuery(this, new EventEventArgs { EventId = eventId });
        }
        private void OnGuestListCreated(Int32 eventId)
        {
            if (GuestListCreated != null)
                GuestListCreated(this, new EventEventArgs { EventId = eventId });
        }
        private void UpdateEvent(EventDTO @event)
        {
            if (@event == null)
                return;

            EditedEvent = new EventDTO(@event);

            OnEventEditingStarted();
        }

        private void DeleteEvent(EventDTO Event)
        {
            if (Event == null || !Events.Contains(Event))
                return;

            Events.Remove(Event);

            model.DeleteEvent(Event);
        }

        private void UpdateVenue(VenueDTO venue)
        {
            if (venue == null)
                return;

            EditedVenue = new VenueDTO(venue);

            OnVenueEditingStarted();
        }

        private void DeleteVenue(VenueDTO venue)
        {
            if (venue == null)
                return;

            EventDTO eventDTO = Events.FirstOrDefault(e => e.Id == venue.EventId);
            if (eventDTO == null)
                return;

            model.DeleteVenue(venue);
            eventDTO.Venues.Remove(venue);
            SelectedEvent = eventDTO;
            SelectedVenue = null;
        }

        private void DeleteImage(VenueImageDTO image)
        {
            if (image == null)
                return;

            model.DeleteImage(image);
        }

        private void SaveEventChanges()
        {
            if (String.IsNullOrEmpty(EditedEvent.Name))
            {
                OnMessageApplication("Name for event is required.");
                return;
            }
            if (EditedEvent.DeadlineForApplication == null)
            {
                OnMessageApplication("Deadline for application is required.");
                return;
            }
            if (EditedEvent.StartDate == null)
            {
                OnMessageApplication("Start date is required.");
                return;
            }
            if (EditedEvent.EndDate == null)
            {
                OnMessageApplication("End date is required.");
                return;
            }
            if (EditedEvent.EndDate < EditedEvent.StartDate ||
                EditedEvent.EndDate < EditedEvent.DeadlineForApplication || 
                EditedEvent.StartDate < EditedEvent.DeadlineForApplication)
            {
                OnMessageApplication("The order of the dates should be: deadline for application, start date, end date.");
                return;
            }

            if (EditedEvent.Id == 0)
            {
                model.CreateEvent(EditedEvent);
                Events.Add(EditedEvent);
                SelectedEvent = EditedEvent;
            }
            else
            {
                model.UpdateEvent(EditedEvent);
            }

            EditedEvent = null;

            OnEventEditingFinished();
        }


        private void SaveVenueChanges()
        {
            if (String.IsNullOrEmpty(EditedVenue.Name))
            {
                OnMessageApplication("Name for event is required.");
                return;
            }

            if (EditedVenue.Id == 0)
            {
                model.CreateVenue(EditedVenue);
                SelectedVenue = EditedVenue;
            }
            else
            {
                model.UpdateVenue(EditedVenue);
            }

            EditedVenue = null;

            OnVenueEditingFinished();
        }



        private void CancelEventChanges()
        {
            EditedEvent = null;
            OnEventEditingFinished();
        }

        private void CancelVenueChanges()
        {
            EditedVenue = null;
            OnVenueEditingFinished();
        }


        private async void LoadAsync(String organisationName)
        {
            try
            {
                await model.LoadAsync();
                Events = new ObservableCollection<EventDTO>(model.Events);
                Members = new ObservableCollection<MemberDTO>(model.Members);
                IsLoaded = true;
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("Loading failed. No connection with provider.");
            }
        }

        private async void SaveAsync()
        {
            try
            {
                await model.SaveAsync();
                OnMessageApplication("Saved successfully.");
            }
            catch (PersistenceUnavailableException)
            {
                OnMessageApplication("Save failed. No connection with provider.");
            }
        }

        private void Model_EventChanged(object sender, EventEventArgs e)
        {
            Int32 index = Events.IndexOf(Events.FirstOrDefault(Event => Event.Id == e.EventId));
            Events.RemoveAt(index);
            Events.Insert(index, model.Events[index]);

            SelectedEvent = Events[index];
        }

        private void Model_VenueChanged(object sender, VenueEventArgs e)
        {
            Int32 index = Events.IndexOf(Events.FirstOrDefault(Event => Event.Id == e.EventId));
            Events.RemoveAt(index);
            Events.Insert(index, model.Events[index]);

            SelectedVenue = Events[index].Venues.First(v => v.Id == e.VenueId);
            SelectedEvent = Events[index];
        }

        private void OnExitApplication()
        {
            if (ExitApplication != null)
                ExitApplication(this, EventArgs.Empty);
        }


        private void OnEventEditingStarted()
        {
            if (EventEditingStarted != null)
                EventEditingStarted(this, EventArgs.Empty);
        }

        private void OnEventEditingFinished()
        {
            if (EventEditingFinished != null)
                EventEditingFinished(this, EventArgs.Empty);
        }

        private void OnVenueEditingStarted()
        {
            if (VenueEditingStarted != null)
                VenueEditingStarted(this, EventArgs.Empty);
        }

        private void OnVenueEditingFinished()
        {
            if (VenueEditingFinished != null)
                VenueEditingFinished(this, EventArgs.Empty);
        }

        private void OnImageEditingStarted(Int32 eventId, Int32 venueId)
        {
            if (ImageEditingStarted != null)
                ImageEditingStarted(this, new VenueEventArgs { VenueId = venueId, EventId = eventId });
        }
    }
}
