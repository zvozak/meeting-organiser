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
using CommonData.DTOs;
using NLog;

namespace ServicesForDesktopApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public ProjectsController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }


        [HttpGet("{organisationId}")]
        public ActionResult<IEnumerable<ProjectDTO>> GetProjectsOfOrganisation(int organisationId)
        {
            return Ok(context.Projects
                .Where(e => e.OrganisationId == organisationId)
                .Select(e => (ProjectDTO)e));
        }


        [HttpGet("{memberId}")]
        public ActionResult<IEnumerable<ProjectDTO>> GetProjectsOfMember(int memberId)
        {
            return Ok(context.Projects
                .Where(e =>  context.MemberOfProjects
                .Any( m => e.Id == m.ProjectId && m.MemberId == memberId))
                .Select(e => (ProjectDTO)e));
        }


        [HttpGet("{id}")]
        public ActionResult<ProjectDTO> GetProject(int id)
        {
            var project = context.Projects.Find(id);

            if (project == null)
            {
                return NotFound();
            }

            return (ProjectDTO)project;
        }


        [HttpPut]
        [Authorize(Roles = "administrator")]
        public IActionResult PutProject([FromBody] ProjectDTO projectDTO)
        {
            try
            {
                Project project = context.Projects.First(e => e.Id == projectDTO.Id);

                project = (Project)projectDTO;
                project.Id = projectDTO.Id;
                context.SaveChanges();

                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "PROJECT PUT query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(projectDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        [Authorize(Roles = "administrator")]
        public IActionResult PostProject(ProjectDTO eventDTO)
        {
            context.Projects.Add((Project)eventDTO);
            try
            {
                context.SaveChanges();

                return CreatedAtAction(nameof(PostProject), new { id = eventDTO.Id }, eventDTO);
            }
            catch (Exception exception)
            {
                log.Error(exception, "PROJECT POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(eventDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteProject(int id)
        {
            try
            {
                var project = context.Projects.First(m => m.Id == id);
                context.Projects.Remove(project);
                context.SaveChanges();
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception exception)
            {
                log.Error(exception, "PROJECT DELETE query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress, id);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
