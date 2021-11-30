using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommonData;
using CommonData.Entities;
using NLog;
using CommonData.DTOs;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ServicesForDesktopApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public EventsController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }


        [HttpGet("{eventId}")]
        public ActionResult<IEnumerable<EventFormDTO>> GetEventForms(int eventId)
        {
            try {
                return Ok(context.EventForms
                    .Where(e => e.EventId == eventId)
                    .Select(e => (EventFormDTO)e));
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }


        [HttpGet("{organisationId}")]
        public ActionResult<IEnumerable<EventDTO>> GetEvents(int organisationId)
        {
            try
            {
                return Ok(context.Events
                    .Where(e => e.OrganisationId == organisationId)
                    .Select(e => (EventDTO)e));
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }


        [HttpGet("{id}")]
        public ActionResult<EventDTO> GetEvent(int id)
        {
            var @event = context.Events.Find(id);

            if (@event == null)
            {
                return NotFound();
            }

            return (EventDTO)@event;
        }

        
        [HttpPut]
        [Authorize(Roles = "administrator")]
        public IActionResult PutEvent([FromBody] EventDTO eventDTO)
        {
            try
            {
                Event @event = context.Events.First(e => e.Id == eventDTO.Id);

                @event = (Event)eventDTO;
                @event.Id = eventDTO.Id;

                context.SaveChanges();

                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "EVENT PUT query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(eventDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [Authorize(Roles = "administrator")]
        public IActionResult PostEvent(EventDTO eventDTO)
        {
            Event newEvent = (Event)eventDTO;
            context.Events.Add(newEvent);
            try
            {
                context.SaveChanges();

                return CreatedAtAction(nameof(PostEvent), new { id = eventDTO.Id }, eventDTO);
            }
            
            catch (Exception exception)
            {
                log.Error(exception, "EVENT POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(eventDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteEvent(int id)
        {
            try
            {
                var @event = context.Events.First(m => m.Id == id);
                context.Events.Remove(@event);
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "EVENT DELETE query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress, id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
