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
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace ServicesForDesktopApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public JobsController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }


        [HttpGet("{organisationId}")]
        public ActionResult<IEnumerable<JobDTO>> GetJobs(int organisationId)
        {
            return Ok(context.Jobs
                .Where(e => e.OrganisationId == organisationId)
                .Select(e => (JobDTO)e));
        }


        [HttpGet("{jobId}")]
        public ActionResult<JobDTO> GetJob(int jobId)
        {
            var job = context.Jobs.Find(jobId);

            if (job == null)
            {
                return NotFound();
            }

            return (JobDTO)job;
        }


        [HttpPut]
        [Authorize(Roles = "administrator")]
        public IActionResult PutJob([FromBody] JobDTO jobDTO)
        {
            try
            {
                Job job = context.Jobs.First(e => e.Id == jobDTO.Id);

                job = (Job)jobDTO;
                job.Id = jobDTO.Id;
                context.SaveChanges();

                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "JOB PUT query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(jobDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [Authorize(Roles = "administrator")]
        public IActionResult PostJob(JobDTO jobDTO)
        {
            context.Jobs.Add((Job)jobDTO);
            try
            {
                context.SaveChanges();

                return CreatedAtAction(nameof(PostJob), new { id = jobDTO.Id }, jobDTO);
            }
            catch (Exception exception)
            {
                log.Error(exception, "JOB POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(jobDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteJob(int id)
        {
            try
            {
                var job = context.Jobs.First(m => m.Id == id);
                context.Jobs.Remove(job);
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "JOB DELETE query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress, id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
