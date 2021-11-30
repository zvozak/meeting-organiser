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
    public class VenueImagesController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public VenueImagesController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }


        [HttpGet("{venueId}")]
        public ActionResult<IEnumerable<VenueImageDTO>> GetVenueImages(int venueId)
        {
            return Ok(context.VenueImages
                .Where(e => e.VenueId == venueId)
                .Select(e => (VenueImageDTO)e));
        }


        [HttpGet("{id}")]
        public ActionResult<VenueImageDTO> GetVenueImage(int id)
        {
            var image = context.VenueImages.Find(id);

            if (image == null)
            {
                return NotFound();
            }

            return (VenueImageDTO)image;
        }

        [HttpPost]
        [Authorize(Roles = "administrator")]
        public IActionResult PostVenueImage(VenueImageDTO eventDTO)
        {
            context.VenueImages.Add((VenueImage)eventDTO);
            try
            {
                context.SaveChanges();

                return CreatedAtAction(nameof(PostVenueImage), new { id = eventDTO.Id }, eventDTO);
            }
            catch (Exception exception)
            {
                log.Error(exception, "VENUEIMAGE POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(eventDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteVenueImage(int id)
        {
            try
            {
                var image = context.VenueImages.First(m => m.Id == id);
                context.VenueImages.Remove(image);
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "VENUEIMAGE DELETE query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress, id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
