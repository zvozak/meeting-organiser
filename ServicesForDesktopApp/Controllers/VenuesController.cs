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
    public class VenuesController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public VenuesController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }


        [HttpGet("{eventId}")]
        public ActionResult<IEnumerable<VenueDTO>> GetVenues(int eventId)
        {
            return Ok(context.Venues
                .Where(e => e.EventId == eventId)
                .Select(e => (VenueDTO)e));
        }


        [HttpGet("{id}")]
        public ActionResult<VenueDTO> GetVenue(int id)
        {
            var venue = context.Venues.Find(id);

            if (venue == null)
            {
                return NotFound();
            }

            return (VenueDTO)venue;
        }


        [HttpPut]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> PutVenue([FromBody] VenueDTO venueDTO)
        {
            try
            {
                Venue venue = context.Venues.First(e => e.Id == venueDTO.Id);

                venue = (Venue)venueDTO;
                venue.Id = venueDTO.Id;
                context.SaveChanges();

                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "VENUE PUT query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(venueDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [Authorize(Roles = "administrator")]
        public ActionResult<VenueDTO> PostVenue(VenueDTO eventDTO)
        {
            context.Venues.Add((Venue)eventDTO);
            try
            {
                context.SaveChanges();

                return CreatedAtAction("GetVenue", new { id = eventDTO.Id }, eventDTO);
            }
            catch (Exception exception)
            {
                log.Error(exception, "VENUE POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(eventDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteVenue(int id)
        {
            try
            {
                var venue = context.Venues.First(m => m.Id == id);
                context.Venues.Remove(venue);
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "VENUE DELETE query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress, id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
