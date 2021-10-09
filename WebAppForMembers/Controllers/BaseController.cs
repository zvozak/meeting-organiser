using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using WebAppForMembers.Models;
using Microsoft.AspNetCore.Identity;
using CommonData.Entities;
using System.Threading.Tasks;

namespace WebAppForMembers.Controllers
{
	public class BaseController : Controller
	{
		protected readonly IServiceForMembers service;
		protected readonly UserManager<User> userManager;

		public BaseController(IServiceForMembers service, UserManager<User> userManager)
		{
			this.service = service;
			this.userManager = userManager;
		}

		/// <summary>
		/// Egy akció meghívása után végrehajtandó metódus.
		/// </summary>
		/// <param name="context">Az akció kontextus argumentuma.</param>
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			base.OnActionExecuted(context);

			ViewData["CurrentUserName"] = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
		}

		protected async Task<User> GetCurrentUser()
        {
			if (User.Identity.IsAuthenticated)
			{
				User currentUser = await userManager.FindByNameAsync(User.Identity.Name);

				if (currentUser != null)
				{
					return currentUser;
				}
				else
				{
					throw new Exception("Could not load data of current user.");
				}
			}
			else
			{
				throw new Exception("Page should be reached only by authenticated users.");
			}
		}
	}
}
