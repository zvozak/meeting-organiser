using System;
using System.Collections.Generic;
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
    public class MembersController : ControllerBase
    {
        private readonly MeetingApplicationContext context;
        private readonly Logger log;

        public MembersController(MeetingApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.context = context;
            log = LogManager.GetLogger("service");
        }

        [HttpGet("{organisationId}")]
        [Authorize(Roles = "administrator")]
        public ActionResult<IEnumerable<MemberDTO>> GetMembers(int organisationId)
        {
            return Ok (context.Members
                .Where (member => member.OrganisationId == organisationId)
                .Select (member => (MemberDTO)member));
        }

        [HttpGet("{memberId}")]
        [Authorize(Roles = "administrator")]
        public ActionResult<MemberDTO> GetMember(int memberId)
        {
            var member = context.Members.Find(memberId);

            if (member == null)
            {
                return NotFound();
            }

            return (MemberDTO)member;
        }

        [HttpPut]
        [Authorize(Roles = "administrator")]
        public IActionResult PutMember ([FromBody] MemberDTO memberDTO)
        {
            try{
                Member member = context.Members.First(m => m.Id == memberDTO.Id);

                member = (Member)memberDTO;
                member.Id = memberDTO.Id;
                context.SaveChanges();

                return Ok();
            }
            catch (ArgumentNullException){
                return NotFound();
            }
            catch (Exception exception) {
                log.Error(exception, "MEMBER PUT query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(memberDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        [HttpPost]
        [Authorize(Roles = "administrator")]
        public IActionResult PostMember([FromBody] MemberDTO memberDTO)
        {
            if (memberDTO.BossId == null && memberDTO.Boss != null)
            {
                var boss = context.Members.FirstOrDefault(m => m.Name == memberDTO.Boss.Name);
                if (boss != null)
                {
                    memberDTO.BossId = boss.Id;
                }
            }
            if (memberDTO.JobId == null && memberDTO.Job != null)
            {
                memberDTO.JobId = context.Jobs.First(j => j.Title == memberDTO.Job.Title && j.OrganisationId == memberDTO.OrganisationId).Id;
            }

            var newMember = new Member
            {
                Name = memberDTO.Name,
                Email = memberDTO.Email,
                Department = memberDTO.Department,
                DateOfJoining = memberDTO.DateOfJoining,
                IdAtOrganisation = memberDTO.IdAtOrganisation,
                OrganisationId = memberDTO.OrganisationId,
                BossId = memberDTO.BossId,
                JobId = memberDTO.JobId
            };
            
            context.Members.Add (newMember);
            try
            {
                context.SaveChanges();

                var actionName = nameof(PostMember);

                memberDTO.Id = newMember.Id;

                var result = CreatedAtAction(
                    actionName: actionName,
                    routeValues: new { id = memberDTO.Id },
                    value: memberDTO);
                return result;
            }
            catch (Exception exception)
            {
                log.Error(exception, "MEMBER POST query failed from address {0} with content: {1}",
                    HttpContext.Connection.RemoteIpAddress,
                    JsonConvert.SerializeObject(memberDTO, Formatting.None));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public ActionResult DeleteMember(int id)
        {
            try
            {
                var member = context.Members.First(m => m.Id == id);
                context.Members.Remove(member);
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
