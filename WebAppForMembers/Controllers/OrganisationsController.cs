using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommonData;
using CommonData.Entities;
using WebAppForMembers.Models;
using Microsoft.AspNetCore.Identity;

namespace WebAppForMembers.Controllers
{
    public class OrganisationsController : BaseController
    {
        public enum SortOrder
        {
            Name_ASC,
            Name_DESC,
            NumberOfEvents_ASC,
            NumberOfEvents_DESC,
            Deadline
        }


        public OrganisationsController(IServiceForMembers service, UserManager<User> userManager) : base(service, userManager)
        {
        }

        // GET: Organisations
        public async Task<IActionResult> Index(SortOrder sortOrder = SortOrder.Name_ASC)
        {
            int idOfCurrentUser;
            if (User.Identity.IsAuthenticated)
            {
                User currentUser = await userManager.FindByNameAsync(User.Identity.Name);

                if (currentUser != null)
                {
                    idOfCurrentUser = currentUser.Id;
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

            HashSet<Organisation> organisations = service.GetOrganisationsOf(idOfCurrentUser).ToHashSet();
            HashSet<OrganisationViewModel> organisationViews = new HashSet<OrganisationViewModel>(organisations.Count);
            foreach (var org in organisations)
            {
                organisationViews.Add(new OrganisationViewModel
                {
                    Id = org.Id,
                    Name = org.Name,
                    Description = org.Description,
                    Deadline = service.StrictestDeadlineAt(org.Id),
                    NumberOfEvents = service.GetNumberOfEventsIn(org.Name)
                });
            }


            ViewData["SortNameParam"] = 
                sortOrder == SortOrder.Name_ASC
                ? SortOrder.Name_DESC : SortOrder.Name_ASC;
            ViewData["SortNumberOfEventsParam"] = 
                sortOrder == SortOrder.NumberOfEvents_ASC 
                ? SortOrder.NumberOfEvents_DESC : SortOrder.NumberOfEvents_ASC;
            ViewData["SortDeadlineParam"] = SortOrder.Deadline;

            switch (sortOrder)
            {
                case SortOrder.Name_DESC:
                    return View(
                        organisationViews
                        .OrderByDescending(o => o.Name));
                case SortOrder.NumberOfEvents_ASC:
                    return View(
                        organisationViews
                        .OrderBy(o => o.NumberOfEvents));
                case SortOrder.NumberOfEvents_DESC:
                    return View(
                        organisationViews
                        .OrderByDescending(o => o.NumberOfEvents));
                case SortOrder.Deadline:
                    return View(
                        organisationViews
                        .OrderBy( o => o.Deadline));
                default:
                    return View(
                        organisationViews
                        .OrderBy(o => o.Name));
            }
        }

        // GET: Organisations/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Organisation organisation = service.GetOrganisation((int)id);
            if (organisation == null)
            {
                return NotFound();
            }
            List<EventViewModel> events = service.GetEventsOf((int)id).ToList();

            return View(new OrganisationDetailViewModel
            {
                Organisation = organisation,
                Events = events
            });
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Organisation organisation = service.GetOrganisation((int)id);

            if (organisation == null)
            {
                return NotFound();
            }

            return View(
                new OrganisationViewModel
                {
                    Id = organisation.Id,
                    Name = organisation.Name,
                    Description = organisation.Description,
                    Deadline = service.StrictestDeadlineAt(organisation.Id),
                    NumberOfEvents = service.GetNumberOfEventsIn(organisation.Name)
                }
            );
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            User user = GetCurrentUser().Result;

            if (user == null)
            {
                return NotFound();
            }

            if (service.DeleteMembership(user.Id, id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("ModificationResult", new ModificationResultViewModel
                {
                    Action = "Index",
                    Controller = "Organisations",
                    Message = "An error occured when trying to quit organisation."
                });
            }
        }

    }
}
