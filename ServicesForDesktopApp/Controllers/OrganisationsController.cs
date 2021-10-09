using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommonData;
using CommonData.Entities;
using CommonData.DTOs;
using NLog;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace ServicesForDesktopApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrganisationsController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public OrganisationsController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }


        [Authorize(Roles = "administrator")]
        [HttpGet("{organisationName}")]
        public IActionResult GetOrganisation(string organisationName)
        {
            try
            {
                var organisation = context.Organisations.Single(o => o.Name == organisationName);
                OrganisationDTO result = (OrganisationDTO) organisation;

                return Ok(result);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }            
        }

        /*
        [Authorize(Roles = "administrator")]
        [HttpGet("{id}")]
        public ActionResult<Organisation> GetOrganisation(int id)
        {
            var organisation = context.Organisations.Find(id);

            if (organisation == null)
            {
                return NotFound();
            }

            return organisation;
        }
        */

        [HttpPut]
        [Authorize(Roles = "administrator")]
        public IActionResult PutOrganisation(OrganisationDTO organisationDTO)
        {
            try
            {
                Organisation organisation = context.Organisations.First(o => o.Id == organisationDTO.Id);
                organisation = (Organisation)organisationDTO;
                organisation.Id = organisationDTO.Id;
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "MEMBER PUT query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(organisationDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [Authorize(Roles = "administrator")]
        public ActionResult<OrganisationDTO> PostOrganisation(OrganisationDTO organisationDTO)
        {
            context.Organisations.Add((Organisation)organisationDTO);

            try
            {
                context.SaveChanges();
                return CreatedAtAction("GetOrganisation", new { id = organisationDTO.Id }, organisationDTO);
            }
            catch (Exception exception)
            {
                log.Error(exception, "MEMBER POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(organisationDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteOrganisation(int id)
        {
            try
            {
                var organisation = context.Organisations.First(o => o.Id == id);
                context.Organisations.Remove(organisation);
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "MEMBER DELETE query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress, id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
