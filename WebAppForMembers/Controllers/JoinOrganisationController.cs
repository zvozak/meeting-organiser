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
    public class JoinOrganisationController : BaseController
    {
        public JoinOrganisationController(IServiceForMembers service, UserManager<User> userManager) : base(service, userManager)
        {
        }

        public JsonResult VerifyOrganisation(string OrganisationName)
        {
            var currentUserName = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
            if (currentUserName == null)
            {
                throw new Exception("User not found.");
            }
            int currentUserId = service.GetUserBy(currentUserName).Id;
            
            return Json(service.IsExistingOrganisation(OrganisationName));
        }
        public JsonResult VerifyMemberBy(string EmailAddressAtOrganisation, string OrganisationName)
        {
            var currentUserName = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
            if(currentUserName == null)
            {
                throw new Exception("User not found.");
            }
            User currentUser = service.GetUserBy(currentUserName);

            if( service.IsMemberOfOrganisation(
                    OrganisationName,
                    realUserName: currentUser.Name,
                    userEmail: EmailAddressAtOrganisation))
            {
                int memberId = service.GetMemberIdBy(OrganisationName, currentUser.Name, EmailAddressAtOrganisation);
                return Json(!service.IsExistingMembership(memberId, currentUser.Id));
            }
            else
            {
                return VerifyEmail(EmailAddressAtOrganisation, OrganisationName);
            }
        }
        public JsonResult VerifyBoss( string emailOfBoss, string nameOfBoss, string organisationName)
        {
            return Json( IsBossValid( nameOfBoss, emailOfBoss, organisationName));
        }

        public JsonResult VerifyEmail(string Email, string OrganisationName)
        {
            return Json(!service.IsEmailAtOrganisation(
                    OrganisationName, Email) &&
                    service.CanJoinOrganisation(OrganisationName, Email));
        }
        public JsonResult VerifyMemberIsNew(string EmailAddressAtOrganisation, string OrganisationName)
        {
            var currentUserName = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
            if (currentUserName == null)
            {
                throw new Exception("User not found.");
            }
            User currentUser = service.GetUserBy(currentUserName);

            return Json(service.IsMemberOfOrganisation(
                    OrganisationName,
                    realUserName: currentUser.Name,
                    userEmail: EmailAddressAtOrganisation));
        }

        public JsonResult VerifyProject(string OtherProjectName, IList<Project> PossibleProjectsAtOrganisation, HashSet<string> OtherProjectNames )
        {
            return Json(
                !OtherProjectNames.Contains(OtherProjectName) &&
                !PossibleProjectsAtOrganisation.Any(p => p.Name == OtherProjectName));
        }

        private bool IsBossValid(string nameOfBoss, string emailOfBoss, string OrganisationName)
        {
             return
                nameOfBoss != null &&
                nameOfBoss != "" &&
                emailOfBoss != null &&
                emailOfBoss != "" &&
                service.IsMemberOfOrganisation(
                    OrganisationName,
                    nameOfBoss,
                    emailOfBoss);
        }
        public IActionResult Index()
        {
            return RedirectToAction("Join");
        }

        [HttpGet]
        public IActionResult Join()
        {
            return View("Join");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinViewModel joinModel)
        {
            if (!ModelState.IsValid)
                return View("Join", joinModel);

            var currentUserName = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
            if (currentUserName == null)
            {
                return View(
                    "Error",
                    new ModificationResultViewModel()
                    {
                        Message = "Unlogged users cannot join organisations."
                    });
            }

            User currentUser = service.GetUserBy(currentUserName);
            if (joinModel.EmailAddressAtOrganisation == null ||
                !service.IsMemberOfOrganisation(
                    joinModel.OrganisationName,
                    realUserName: currentUser.Name,
                    userEmail: joinModel.EmailAddressAtOrganisation))
            {
                return RedirectToAction("CreateMember", joinModel);
            }

            bool successful = service.CreateMembership(joinModel.OrganisationName, currentUser.Id, joinModel.EmailAddressAtOrganisation);
            if (!successful)
            {
                return View(
                    "Error",
                    new ModificationResultViewModel()
                    {
                        Message = "Could not join organisation."
                    });
            }

            ModificationResultViewModel success = new ModificationResultViewModel
            {
                Controller = "JoinOrganisation",
                Action = "Join",
                Message = String.Format("You have successfully joined {0}", joinModel.OrganisationName)
            };
            return View("ModificationResult", success);
        }
        
        [HttpGet]
        public IActionResult CreateMember(JoinViewModel joinModel)
        {
            Organisation organisation = service.GetOrganisationByName(joinModel.OrganisationName);
            if(organisation == null)
            {
                return View(new CreateMemberViewModel());
            }
            CreateMemberViewModel model = new CreateMemberViewModel
            {
                OrganisationId = organisation.Id,
                OrganisationName = organisation.Name,
                OrganisationTypeOfStructure = organisation.TypeOfStructure,
                PossibleJobsAtOrganisation = service.PossibleJobsAtOrganisation(organisation.Id).ToList(),
                PossibleProjectsAtOrganisation = service.PossibleProjectsAtOrganisation(organisation.Id).ToList(),
                Email = joinModel.EmailAddressAtOrganisation,
                IdsOfSelectedProjects = new List<int>()
            };
            return View("CreateMember", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMember(CreateMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateMember", model);
            }

            if (model.OrganisationTypeOfStructure == TypeOfStructure.Hierarchical &&
                !IsBossValid(model.NameOfBoss, model.EmailOfBoss, model.OrganisationName))
            {
                ModelState.AddModelError("", "Both name and adress of your supervisor are required.");
                return View("CreateMember", model);
            }

            var currentUserName = String.IsNullOrEmpty(User.Identity.Name) ? null : User.Identity.Name;
            User currentUser = service.GetUserBy(currentUserName);
            bool successful;

            if (currentUserName == null)
            {
                ModelState.AddModelError("", "Unlogged users cannot join organisations.");
                return View("CreateMember", model);
            }

            if (model.IdOfSelectedJob == 0)
            {
                successful = service.CreateJob(model.OtherJobTitle, model.OrganisationId);
                if (!successful)
                {
                    ModelState.AddModelError("", string.Format("Could not create job {0}.\n", model.OtherJobTitle));
                    return View("CreateMember", model);
                }
                else
                {
                    model.IdOfSelectedJob = service.GetJob(model.OtherJobTitle, model.OrganisationId).Id;
                }
            }

            int? bossId = null;
            if (model.OrganisationTypeOfStructure == TypeOfStructure.Hierarchical)
            {
                Member boss = service.GetMember(model.OrganisationName, model.NameOfBoss, model.EmailOfBoss);
                bossId = boss.Id;
            }
            
            Member member = new Member
            {
                DateOfJoining = model.DateOfJoining,
                BossId = bossId,
                Department = model.Department,
                Email = model.Email,
                Name = currentUser.Name,
                JobId = model.IdOfSelectedJob,
                OrganisationId = model.OrganisationId
            };
            successful = service.AddMember(member);
            if (!successful)
            {
                ModelState.AddModelError("", "Could not save data of member.");
                return View("CreateMember", model);
            }

            successful = service.CreateMembership(model.OrganisationName, currentUser.Id, model.Email);
            if (!successful)
            {
                ModelState.AddModelError("", "Could not create membership.");
                return View("CreateMember", model);
            }

            int memberId = service.GetMemberIdBy(model.OrganisationName, currentUser.Name, model.Email);

            string ErrorReport = "";

            if (model.OrganisationTypeOfStructure == TypeOfStructure.ProjectBased)
            {
                if(model.IdsOfSelectedProjects != null)
                {
                    foreach (var projectId in model.IdsOfSelectedProjects)
                    {
                        successful = service.CreateMemberOfProject(memberId, projectId);
                        if (!successful)
                        {
                            ErrorReport += string.Format("Could not add member to project {0}.\n", projectId);
                        }
                    }
                }
                if (model.IdsOfSelectedProjects != null)
                {
                    foreach (var project in model.OtherProjectNames)
                    {
                        successful = service.CreateProject(project, model.OrganisationId);
                        if (!successful)
                        {
                            ErrorReport += string.Format("Could not create project with name {0}.\n", project);
                        }
                        else
                        {
                            int projectId = service.GetProject(project, model.OrganisationId).Id;
                            successful = service.CreateMemberOfProject(memberId, projectId);
                            if (!successful)
                            {
                                ErrorReport += string.Format("Could not add member to project {0}.\n", project);
                            }
                        }
                    }
                }
            }
            
            if(ErrorReport.Length > 0)
            {
                ModelState.AddModelError("", "Successfully joined organisation.\nSome problems occured:\n" + ErrorReport);
                return View("CreateMember", model);
            }

            return RedirectToAction("Index", "Organisations");
        }
        


    }
}
