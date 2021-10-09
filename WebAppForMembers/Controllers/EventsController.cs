using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommonData;
using CommonData.Entities;
using WebAppForMembers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAppForMembers.Controllers
{
    public class EventsController : BaseController
    {
        public enum SortOrder
        {
            Name_ASC,
            Name_DESC,
            StartDate_ASC,
            StartDate_DESC,
            EndDate_ASC,
            EndDate_DESC,
            Deadline
        }

        private readonly IOptions<GoogleConfig> googleConfig;

        public EventsController(IServiceForMembers service, UserManager<User> userManager, IOptions<GoogleConfig> googleConfig) : base(service, userManager)
        {
            this.googleConfig = googleConfig;
        }
        private bool IsBetween(DateTime date, DateTime start, DateTime end)
        {
            return 
                DateTime.Compare(start, date) <= 0 &&
                DateTime.Compare(date, end) <= 0;
        }
        public JsonResult VerifyEndDate(DateTime EndDate, DateTime StartDate, DateTime EventEndDate, DateTime EventStartDate)
        {
            return Json (IsValidEndDate (EndDate, StartDate, EventEndDate, EventStartDate));
        }
        public JsonResult VerifyStartDate(DateTime StartDate, DateTime EndDate, DateTime EventEndDate, DateTime EventStartDate)
        {
            return Json (IsValidStartDate (StartDate, EndDate, EventEndDate, EventStartDate));
        }
        private bool IsValidEndDate(DateTime EndDate, DateTime StartDate, DateTime EventEndDate, DateTime EventStartDate)
        {
            bool a = IsBetween(EndDate, EventStartDate, EventEndDate);
            bool b = (StartDate == null || DateTime.Compare(StartDate, EndDate) < 0);
            return 
                IsBetween(EndDate, EventStartDate, EventEndDate) &&
                (StartDate == null || DateTime.Compare(StartDate, EndDate) < 0);

        }
        private bool IsValidStartDate(DateTime StartDate, DateTime EndDate, DateTime EventEndDate, DateTime EventStartDate)
        {
            bool a = IsBetween(StartDate, EventStartDate, EventEndDate);
            bool b = (EndDate == null || DateTime.Compare(StartDate, EndDate) < 0);
            return 
                IsBetween(StartDate, EventStartDate, EventEndDate) &&
                (EndDate == null || DateTime.Compare(StartDate, EndDate) < 0);

        }


        // GET: EventInfos
        public IActionResult Index(SortOrder sortOrder = SortOrder.Name_ASC)
        {
            User currentUser = GetCurrentUser().Result;

            ViewData["SortNameParam"] =
                sortOrder == SortOrder.Name_ASC
                ? SortOrder.Name_DESC : SortOrder.Name_ASC;
            ViewData["SortStartDateParam"] =
                sortOrder == SortOrder.StartDate_ASC
                ? SortOrder.StartDate_DESC : SortOrder.StartDate_ASC;
            ViewData["SortEndDateParam"] =
                sortOrder == SortOrder.EndDate_ASC
                ? SortOrder.EndDate_DESC : SortOrder.EndDate_ASC;
            ViewData["SortDeadlineParam"] = SortOrder.Deadline;

            var events = service.GetAvailableEventsOf(currentUser.Id);

            switch (sortOrder)
            {
                case SortOrder.Name_DESC:
                    return View(
                        events
                        .OrderByDescending(o => o.Name));
                case SortOrder.StartDate_ASC:
                    return View(
                        events
                        .OrderBy(o => o.StartDate));
                case SortOrder.StartDate_DESC:
                    return View(
                        events
                        .OrderByDescending(o => o.StartDate));
                case SortOrder.EndDate_ASC:
                    return View(
                        events
                        .OrderBy(o => o.EndDate));
                case SortOrder.EndDate_DESC:
                    return View(
                        events
                        .OrderByDescending(o => o.EndDate));
                case SortOrder.Deadline:
                    return View(
                        events
                        .OrderBy(o => o.DeadlineForApplication));
                default:
                    return View(
                        events
                        .OrderBy(o => o.Name));
            }
        }

        
        // GET: EventInfos/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventInfo = service.GetEventViewModel((int)id);
            if (eventInfo == null)
            {
                return NotFound();
            }

            var viewModel = new EventDetailViewModel
            {
                Event = eventInfo,
                Venues = service.GetVenues((int)id).ToList()
            };
            return View(viewModel);
        }

        public IActionResult VenueDetails(int venueId)
        {
            ViewBag.GoogleMapsApiKey = googleConfig.Value.MapsApiKey;

            var venue = service.GetVenue(venueId);
            if(venue == null)
            {
                return NotFound();
            }

            return View(
                new VenueViewModel
                {
                    Venue = venue,
                    ImageIds = service.GetVenueImageIds(venueId).ToList()
                });
        }

        public FileResult ImageForVenue(Int32? id)
        {
            // lekérjük az épület első tárolt képjét (kicsiben)
            Byte[] imageContent = service.GetVenueMainImage(id);

            if (imageContent == null) // amennyiben nem sikerült betölteni, egy alapértelmezett képet adunk vissza
                return File("~/images/NoImage.png", "image/png");

            return File(imageContent, "image/png");
        }

        public FileResult Image(Int32 id, Boolean large = false)
        {
            Byte[] imageContent = service.GetVenueImage(id, large);

            if (imageContent == null)
                return File("~/images/NoImage.png", "image/png");

            return File(imageContent, "image/png");
        }


        public void Reject(int eventId)
        {
            User user = GetCurrentUser().Result;
            service.RejectEvent(user.Id, eventId);
        }
        public void Accept(int eventId)
        {
            User user = GetCurrentUser().Result;
            service.AcceptEvent(user.Id, eventId);
        }

        // GET: EventInfos/EditForm/5
        public IActionResult EditForm(int eventId)
        {
            Event currentEvent = service.GetEvent(eventId);
            User user = GetCurrentUser().Result;

            int memberId = service.GetMemberIdBy(user.Id, currentEvent.OrganisationId);
            var eventForm = service.GetEventForm(eventId, memberId);
            
            if(eventForm == null)
            {
                eventForm = new EventForm
                {
                    EventId = eventId,
                    MemberId = memberId
                };
            }

            var viewModel = new EventFormViewModel
            {
                EventId = eventForm.EventId,
                MemberId = eventForm.MemberId,
                IsFixGuest = eventForm.IsFixGuest,
                SelectedVenueId = eventForm.SelectedVenueId,
                StartDate = eventForm.StartDate,
                EndDate = eventForm.EndDate,
                Comment = eventForm.Comment,
                EventStartDate = currentEvent.StartDate,
                EventEndDate = currentEvent.EndDate,
                Venues = service.GetVenues(eventId).ToList(),
                WantToJoin = eventForm.WantToJoin
            };

            return View(viewModel);
        }

        // POST: EventInfos/EditForm/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditForm(EventFormViewModel eventFormViewModel)
        {
            string successMessage = "Your response to the event is successfully saved.";
            string errorMessage = "An error occured while saving your response to the event.";
            ModificationResultViewModel resultView = new ModificationResultViewModel
            {
                Action = "Index",
                Controller = "Events"
            };

            var originalEventForm = service.GetEventForm(eventFormViewModel.EventId, eventFormViewModel.MemberId);
            var newEventForm = new EventForm
            {
                EventId = eventFormViewModel.EventId,
                MemberId = eventFormViewModel.MemberId,
                IsFixGuest = eventFormViewModel.IsFixGuest,
                SelectedVenueId = eventFormViewModel.SelectedVenueId,
                StartDate = eventFormViewModel.StartDate,
                EndDate = eventFormViewModel.EndDate,
                Comment = eventFormViewModel.Comment,
                WantToJoin = eventFormViewModel.WantToJoin
            };
            
            if (originalEventForm == null)
            {
                if(service.CreateEventForm(newEventForm))
                {
                    resultView.Message = successMessage;
                }
                else
                {
                    resultView.Message = errorMessage;
                }
            }
            else
            {
                if (service.EditEventForm(newEventForm))
                {
                    resultView.Message = successMessage;
                }
                else
                {
                    resultView.Message = errorMessage;
                }
            }

            return View("ModificationResult", resultView);
            }

    }
}
