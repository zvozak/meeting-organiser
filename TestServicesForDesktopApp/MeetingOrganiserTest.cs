using CommonData.DTOs;
using CommonData.Entities;
using CommonData;
using ServicesForDesktopApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestServicesForDesktopApp
{
    public class MeetingOrganiserTest : IDisposable
    {
        private readonly MeetingApplicationContext context;
        private readonly List<MemberDTO> memberDTOs;
        private readonly List<EventDTO> eventDTOs;
        private readonly List<JobDTO> jobDTOs;
        private readonly List<ProjectDTO> projectDTOs;
        private readonly List<MemberOfProjectDTO> memberOfProjectDTOs;
        private readonly List<AcceptedEmailDomainDTO> emailDomainDTOs;
        private readonly List<VenueDTO> venueDTOs;
        private readonly List<OrganisationDTO> organisationDTOs;

        public MeetingOrganiserTest()
        {
            var options = new DbContextOptionsBuilder<MeetingApplicationContext>()
                .UseInMemoryDatabase("MeetingOrganiserTest")
                .Options;

            context = new MeetingApplicationContext(options);
            context.Database.EnsureCreated();

            var organisationData = new List<Organisation>
            {
                new Organisation
                {
                    Name = "Testorganisation1",
                    Address = "Test utca 1.",
                    AdminId = 1,
                    Description = "Test description1",
                    PermitNewMembers = false,
                    TypeOfStructure = TypeOfStructure.Hierarchical
                },
                new Organisation
                {
                    Name = "Testorganisation2",
                    Address = "Test utca 2.",
                    AdminId = 2,
                    Description = "Test description2",
                    PermitNewMembers = false,
                    TypeOfStructure = TypeOfStructure.ProjectBased
                }
            };
            var jobData = new List<Job>
            {
                new Job
                {
                    OrganisationId = 1,
                    Title = "Test job1",
                    Weight = 1
                },
                new Job
                {
                    OrganisationId = 1,
                    Title = "Test job2",
                    Weight = 2
                },
                new Job
                {
                    OrganisationId = 1,
                    Title = "Test job3",
                    Weight = 3
                },
                new Job
                {
                    OrganisationId = 2,
                    Title = "Test job4",
                    Weight = 4
                },
                new Job
                {
                    OrganisationId = 2,
                    Title = "Test job5",
                    Weight = 5
                }
            };
            var emailDomainData = new List<AcceptedEmailDomain>
            {
                new AcceptedEmailDomain
                {
                    DomainName = "gmail.com",
                    OrganisationId = 1
                },
                new AcceptedEmailDomain
                {
                    DomainName = "gmail.com",
                    OrganisationId = 2
                }
            };
            var projectData = new List<Project>
            {
                new Project
                {
                    Name = "Testproject",
                    OrganisationId = 2,
                    Weight = 1
                }
            };
            var memberData = new List<Member>
            {
                new Member {
                    Name = "Test Member1",
                    DateOfJoining = DateTime.Today,
                    Department = "Test Department",
                    Email = "testemail1@gmail.com",
                    JobId = 1,
                    OrganisationId = 1,
                    IdAtOrganisation = 1
                },
                new Member {
                    Name = "Test Member2",
                    DateOfJoining = DateTime.Today,
                    Department = "Test Department",
                    Email = "testemail2@gmail.com",
                    JobId = 2,
                    OrganisationId = 1,
                    IdAtOrganisation = 2,
                    BossId = 1
                },
                new Member {
                    Name = "Test Member3",
                    DateOfJoining = DateTime.Today,
                    Department = "Test Department",
                    Email = "testemail3@gmail.com",
                    JobId = 3,
                    OrganisationId = 1,
                    IdAtOrganisation = 3,
                    BossId = 1
                },
                new Member {
                    Name = "Test Member4",
                    DateOfJoining = DateTime.Today,
                    Department = "Test Department",
                    Email = "testemail4@gmail.com",
                    JobId = 4,
                    OrganisationId = 2,
                    IdAtOrganisation = 1
                },
                new Member {
                    Name = "Test Member5",
                    DateOfJoining = DateTime.Today,
                    Department = "Test Department",
                    Email = "testemail5@gmail.com",
                    JobId = 2,
                    OrganisationId = 2,
                    IdAtOrganisation = 1,
                }
            };
            var memberOfProjectData = new List<MemberOfProject>
            {
                new MemberOfProject
                {
                    MemberId = 4,
                    ProjectId = 1
                },
                new MemberOfProject
                {
                    MemberId = 4,
                    ProjectId = 2
                }
            };
            var eventData = new List<Event>
            {
                new Event
                {
                    Name = "Testevent1",
                    OrganisationId = 1,
                    DeadlineForApplication = new DateTime(2022, 1, 1, 10, 0, 0),
                    StartDate = new DateTime(2022, 1, 2, 10, 0, 0),
                    EndDate = new DateTime(2022, 1, 3, 10, 0, 0),
                    Description = "Testdescription1",
                    GuestLimit = 10,
                    IsWeightRequired = true,
                    IsConnectedGraphRequired = true,
                    JobWeight = 1,
                    ProjectImportanceWeight = 0,
                    NumberOfNeighboursWeight = 1,
                    NumberOfProjectsWeight = 0,
                    NumberOfSubordinatesWeight = 1                    
                },
                new Event
                {
                    Name = "Testevent2",
                    OrganisationId = 1,
                    DeadlineForApplication = new DateTime(2022, 2, 1, 11, 30, 0),
                    StartDate = new DateTime(2022, 2, 2, 11, 30, 0),
                    EndDate = new DateTime(2022, 2, 3, 11, 30, 0),
                    Description = "Testdescription2",
                    GuestLimit = 10,
                    IsWeightRequired = true,
                    IsConnectedGraphRequired = true,
                    JobWeight = 1,
                    ProjectImportanceWeight = 1,
                    NumberOfNeighboursWeight = 1,
                    NumberOfProjectsWeight = 0,
                    NumberOfSubordinatesWeight = 1
                },
                new Event
                {
                    Name = "Testevent1",
                    OrganisationId = 1,
                    DeadlineForApplication = new DateTime(2022, 3, 1, 10, 0, 0),
                    StartDate = new DateTime(2022, 3, 2, 10, 0, 0),
                    EndDate = new DateTime(2022, 3, 3, 10, 0, 0),
                    Description = "Testdescription3",
                    GuestLimit = 10,
                    IsWeightRequired = true,
                    IsConnectedGraphRequired = true,
                    JobWeight = 1,
                    ProjectImportanceWeight = 1,
                    NumberOfNeighboursWeight = 1,
                    NumberOfProjectsWeight = 0,
                    NumberOfSubordinatesWeight = 1
                }
            };
            var venueData = new List<Venue>
            {
                new Venue
                {
                    EventId = 1,
                    Name = "Testvenue1"
                },
                new Venue
                {
                    EventId = 1,
                    Name = "Testvenue2"
                },
                new Venue
                {
                    EventId = 2,
                    Name = "Testvenue3"
                }
            };

            context.Organisations.AddRange(organisationData);
            context.SaveChanges();
            context.Jobs.AddRange(jobData);
            context.SaveChanges();
            context.AcceptedEmailDomains.AddRange(emailDomainData);
            context.SaveChanges();
            context.Projects.AddRange(projectData);
            context.SaveChanges();
            context.Members.AddRange(memberData);
            context.SaveChanges();
            context.MemberOfProjects.AddRange(memberOfProjectData);
            context.SaveChanges();
            context.Events.AddRange(eventData);
            context.SaveChanges();
            context.Venues.AddRange(venueData);
            context.SaveChanges();

            memberDTOs = memberData.Select(city => new MemberDTO
            {
                Id = city.Id,
                Name = city.Name
            }).ToList();

            organisationDTOs = organisationData.Select(o => (OrganisationDTO) o).ToList();
            jobDTOs = jobData.Select(@job => (JobDTO)@job).ToList();
            emailDomainDTOs = emailDomainData.Select(@emailDomain => (AcceptedEmailDomainDTO)@emailDomain).ToList();
            projectDTOs = projectData.Select(@project => (ProjectDTO)@project).ToList();
            memberDTOs = memberData.Select(@member => (MemberDTO)@member).ToList();
            memberOfProjectDTOs = memberOfProjectData.Select(@memberOfProject => (MemberOfProjectDTO)@memberOfProject).ToList();
            eventDTOs = eventData.Select(@event => (EventDTO)@event).ToList();
            venueDTOs = venueData.Select(@venue => (VenueDTO)@venue).ToList();

            organisationDTOs[0].Jobs.AddRange(jobDTOs.GetRange(0, 3));
            organisationDTOs[1].Jobs.AddRange(jobDTOs.GetRange(3, 2));
            organisationDTOs[1].Projects.Add(projectDTOs[0]);

            memberDTOs[1].Boss = memberDTOs[0];
            memberDTOs[2].Boss = memberDTOs[0];
            memberDTOs[4].Projects.Add(projectDTOs[0]);
            memberDTOs[5].Projects.Add(projectDTOs[0]);

            eventDTOs[0].Venues.AddRange(venueDTOs.GetRange(0, 2));
            eventDTOs[1].Venues.AddRange(venueDTOs.GetRange(2, 1));
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        #region Test Member Controller
        [Fact]
        public void GetMemberTest()
        {
            var controller = new MembersController(context);

            var result = controller.GetMembers(1);
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<MemberDTO>>(objectResult.Value);
            Assert.Equal(memberDTOs.GetRange(0,3), model);

            result = controller.GetMembers(2);
            objectResult = Assert.IsType<OkObjectResult>(result);
            model = Assert.IsAssignableFrom<IEnumerable<MemberDTO>>(objectResult.Value);
            Assert.Equal(memberDTOs.GetRange(3, 2), model);
        }
        [Fact]
        public void CreateMemberTest()
        {
            var newMember = new MemberDTO
            {
                Boss = memberDTOs[0],
                Name = "Test New Member",
                OrganisationId = 1,
                DateOfJoining = DateTime.Today,
                Department = "Test department",
                Email = "testemail6@gmail.com",
                BossId = memberDTOs[0].Id,
                IdAtOrganisation = 4,
                JobId = 1,
                Job = jobDTOs[0]
            };

            var controller = new MembersController(context);
            var result = controller.PostMember(newMember);

            var objectResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsAssignableFrom<MemberDTO>(objectResult.Value);
            Assert.Equal(memberDTOs.Count + 1, context.Members.Count());
            Assert.Equal(newMember, model);
        }

        [Fact]
        public void DeleteMemberTest()
        {
            var controller = new MembersController(context);
            int deletedId = context.Members.First().Id;
            var result = controller.DeleteMember(deletedId);

            Assert.IsType<OkResult>(result);
            Assert.Equal(memberDTOs.Count - 1, context.Members.Count());
            Assert.DoesNotContain(deletedId, context.Members.Select(b => b.Id));
        }
        #endregion

        #region Test Events Controller
        [Fact]
        public void GetEventTest()
        {
            var controller = new EventsController(context);

            var result = controller.GetEvents(1);
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(objectResult.Value);
            Assert.Equal(eventDTOs.GetRange(0,2), model);

            result = controller.GetEvents(2);
            objectResult = Assert.IsType<OkObjectResult>(result);
            model = Assert.IsAssignableFrom<IEnumerable<EventDTO>>(objectResult.Value);
            Assert.Equal(eventDTOs.GetRange(2, 1), model);
        }

        [Fact]
        public void CreateEventTest()
        {
            var newEvent = new EventDTO
            {
                Name = "Testevent1",
                OrganisationId = 1,
                DeadlineForApplication = new DateTime(2022, 1, 1, 10, 0, 0),
                StartDate = new DateTime(2022, 1, 2, 10, 0, 0),
                EndDate = new DateTime(2022, 1, 3, 10, 0, 0),
                Description = "Testdescription1",
                GuestLimit = 10,
                IsWeightRequired = true,
                IsConnectedGraphRequired = true,
                JobWeight = 1,
                ProjectImportanceWeight = 0,
                NumberOfNeighboursWeight = 1,
                NumberOfProjectsWeight = 0,
                NumberOfSubordinatesWeight = 1
            };

            var controller = new EventsController(context);
            var result = controller.PostEvent(newEvent);

            var objectResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsAssignableFrom<EventDTO>(objectResult.Value);
            Assert.Equal(eventDTOs.Count + 1, context.Events.Count());
            Assert.Equal(newEvent, model);
        }

        [Fact]
        public void DeleteEventTest()
        {
            var controller = new EventsController(context);
            int deletedId = context.Events.First().Id;
            var result = controller.DeleteEvent(deletedId);

            Assert.IsType<OkResult>(result);
            Assert.Equal(eventDTOs.Count - 1, context.Events.Count());
            Assert.DoesNotContain(deletedId, context.Events.Select(b => b.Id));
        }
        #endregion


    }
}
