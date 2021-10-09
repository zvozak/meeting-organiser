using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using CommonData.Entities;
using CommonData;

namespace ServiceForAdmins.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AccountController : Controller
    {
        private readonly MeetingApplicationContext context;
        private readonly SignInManager<User> signInManager;

        private readonly Logger log;

        public AccountController(MeetingApplicationContext context, SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
            this.context = context;
            log = LogManager.GetLogger("account");
        }

        [HttpGet("{userName}/{userPassword}/{organisationName}")]
        public async Task<IActionResult> Login(String userName, String userPassword, String organisationName)
        {
            try
            {
                User user = await signInManager.UserManager.FindByNameAsync(userName);
                if (user == null)
                {
                    log.Info("LOGIN FAILED for user {0} from address {1}", userName, HttpContext.Connection.RemoteIpAddress);
                    return Forbid();
                }

                Organisation organisation = context.Organisations.SingleOrDefault(o => 
                    o.Name == organisationName && 
                    o.AdminId == user.Id);

                if(organisation == null)
                {
                    log.Info("LOGIN FAILED for user {0} from address {1}", userName, HttpContext.Connection.RemoteIpAddress);
                    return Forbid();
                }

                var result = await signInManager.PasswordSignInAsync(userName, userPassword, false, false);
                if (!result.Succeeded)
                {
                    log.Info("LOGIN FAILED for user {0} from address {1}", userName, HttpContext.Connection.RemoteIpAddress);
                    return Forbid();
                }

                log.Info("LOGIN SUCCESS for user {0} from address {1}", userName, HttpContext.Connection.RemoteIpAddress);
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}