using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommonData;
using CommonData.Entities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using NLog;
using CommonData.DTOs;

namespace ServicesForDesktopApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AcceptedEmailDomainsController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public AcceptedEmailDomainsController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }


        [HttpGet("{organisationId}")]
        public ActionResult<IEnumerable<AcceptedEmailDomainDTO>> GetAcceptedEmailDomains(int organisationId)
        {
            return Ok(context.AcceptedEmailDomains
                .Where(e => e.OrganisationId == organisationId)
                .Select(e => (AcceptedEmailDomainDTO)e));
        }


        [HttpGet("{id}/{domainName}")]
        public ActionResult<AcceptedEmailDomainDTO> GetAcceptedEmailDomain(int organisationId, string domainName)
        {
            try
            {
                AcceptedEmailDomain domain = context.AcceptedEmailDomains.First(e =>
                e.DomainName == domainName && e.OrganisationId == organisationId);
                return (AcceptedEmailDomainDTO)domain;
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }


        [HttpPut]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> PutAcceptedEmailDomain([FromBody] AcceptedEmailDomainDTO domainDTO)
        {
            try
            {
                AcceptedEmailDomain domain = context.AcceptedEmailDomains.First(e => 
                e.DomainName == domainDTO.DomainName && e.OrganisationId == domainDTO.OrganisationId);

                domain = (AcceptedEmailDomain)domainDTO;
                context.SaveChanges();

                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "EMAILDOMAIN PUT query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(domainDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [Authorize(Roles = "administrator")]
        public IActionResult PostAcceptedEmailDomain(AcceptedEmailDomainDTO domainDTO)
        {
            context.AcceptedEmailDomains.Add((AcceptedEmailDomain)domainDTO);
            try
            {
                context.SaveChanges();

                return CreatedAtAction(nameof(PostAcceptedEmailDomain), new { organisationId = domainDTO.OrganisationId, domainName = domainDTO.DomainName}, domainDTO);
            }
            catch (Exception exception)
            {
                log.Error(exception, "EMAILDOMAIN POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(domainDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}/{domainName}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteAcceptedEmailDomain(int organisationId, string domainName)
        {
            try
            {
                var domain = context.AcceptedEmailDomains
                    .First (m => m.OrganisationId == organisationId && m.DomainName == domainName);

                context.AcceptedEmailDomains.Remove(domain);
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "EMAILDOMAIN DELETE query failed from address {0} with content: {1} - {2}",
                    HttpContext.Connection.RemoteIpAddress, organisationId, domainName);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
