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
        public ActionResult<Member> PostMember(MemberDTO memberDTO)
        {
            context.Members.Add ((Member)memberDTO);
            try
            {
                context.SaveChanges();

                return CreatedAtAction("GetMember", new { id = memberDTO.Id }, memberDTO);
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
