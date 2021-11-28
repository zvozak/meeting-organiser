using CommonData.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppForMembers.Models;

namespace WebAppForMembers.Controllers
{
	public class ForOrganisationsController : BaseController
	{
		private SignInManager<User> signInManager;

		public ForOrganisationsController(IServiceForMembers service,
			UserManager<User> userManager, SignInManager<User> signInManager)
			: base(service, userManager)
		{
			this.signInManager = signInManager;
		}

		public JsonResult VerifyAdminUsername (string AdminUserName)
        {
			return Json (userManager.FindByNameAsync (AdminUserName).Result != null);
        }

		public IActionResult Index()
		{
			return RedirectToAction("Register");
		}


		[HttpGet]
		public IActionResult Register()
		{
			var organisation = new OrganisationRegistrationViewModel();

			return View("Register", organisation);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(OrganisationRegistrationViewModel organisation)
		{
			if (!ModelState.IsValid)
				return View("Register", organisation);


			User admin = await userManager.FindByNameAsync(organisation.AdminUserName);

			if (admin == null)
			{
				ModelState.AddModelError("error", "Username not found.");
				return View("Register", organisation);
			}

			bool successful = service.CreateOrganisation(new Organisation
			{
				Name = organisation.Name,
				TypeOfStructure = organisation.TypeOfStructure,
				Description = organisation.Description,
				Address = organisation.Address,
				AdminId = admin.Id
			});
			
			var currentRoles = userManager.GetRolesAsync(admin).Result;
			IdentityResult result = null;
			if (successful && !currentRoles.Contains("administrator"))
			{
				result = userManager.RemoveFromRolesAsync(admin, currentRoles).Result;
				if (result.Succeeded)
				{
					result = userManager.AddToRoleAsync(admin, "administrator").Result;
				}
			}

			if (successful && (result == null || result.Succeeded))
			{
				return View("ModificationResult", new ModificationResultViewModel
				{
					Action = "Register",
					Controller = "OrganisationRegistration",
					Message = "Account for organisation was successfully created."
				});
			}
			else
            {
				return View("ModificationResult", new ModificationResultViewModel
				{
					Action = "Register",
					Controller = "OrganisationRegistration",
					Message = "An error occured. Could not create account for organisation."
				});
			}
			
		}
	}
}
