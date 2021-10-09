using CommonData.DTOs;
using CommonData.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppForMembers.Models;

namespace WebAppForMembers.Controllers
{
	public class AccountController : BaseController
    {
		private readonly SignInManager<User> signInManager;
		
		public AccountController(IServiceForMembers service,
			UserManager<User> userManager, SignInManager<User> signInManager)
		    : base(service, userManager)
		{
			this.signInManager = signInManager;
		}

		public IActionResult Index()
        {
			return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
			return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
			if (!ModelState.IsValid)
                return View("Login", user);

	        var result = await signInManager.PasswordSignInAsync(user.UserName, user.UserPassword, user.RememberLogin, false);
			if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View("Login", user);
            }

			return RedirectToAction("Index", "Home");
        }

		[HttpGet]
		public IActionResult Register()
		{
			var user = new RegistrationViewModel();
			user.SpecialNeeds = new List<SpecialNeedDTO>();
            for(int i = 0; i< Enum.GetValues(typeof(SpecialNeed)).Length; i++)
            {
				user.SpecialNeeds.Add(new SpecialNeedDTO { Id = i, IsSelected = false });
			}
			return View("Register", user);
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel user)
        {
			if (!ModelState.IsValid)
                return View("Register", user);

			for (int i = 0; i < Enum.GetValues(typeof(SpecialNeed)).Length; i++)
			{
				user.SpecialNeeds[i].Id = i;
			}

			SpecialNeed specialNeeds = SpecialNeedDTO.Convert(
				user.SpecialNeeds.ToArray());

			User guest = new User
	        {
				UserName = user.UserName,
				Email = user.GuestEmail,
				Name = user.GuestName,
				Address = user.GuestAddress,
				PhoneNumber = user.GuestPhoneNumber,
				SpecialNeed = specialNeeds
	        };
	        var result = await userManager.CreateAsync(guest, user.UserPassword);
	        if (!result.Succeeded)
	        {
				foreach (var error in result.Errors)
			        ModelState.AddModelError("", error.Description);
                return View("Register", user);
            }

	        await signInManager.SignInAsync(guest, false);

			service.CreateMemberships(guest.UserName);

			return RedirectToAction("Index", "Home");
		}

        public async Task<IActionResult> Logout()
        {
	        await signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
        }
    }
}