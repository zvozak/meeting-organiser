using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using CommonData;
using WebAppForMembers.Models;
using CommonData.Entities;

namespace TestWebAppForMembers
{
    [TestClass]
    public class TestServiceForMembers
    {
        ServiceForMembers service;
		Mock<UserManager<User>> userManagerMock;
		Mock<MeetingApplicationContext> contextMock;
		int nonexistentOrganisationIdInTestExamples = 100;

        #region TESTS


        [TestMethod]
        public void TestSaveChangesWhenSaveChangesMethodOfContextRaisesErrorShouldReturnFalse()
        {
            contextMock.Setup(c => c.SaveChanges()).Throws(new Exception());

            Assert.IsFalse(service.SaveChanges());
            contextMock.Verify(c => c.SaveChanges(), Times.Once());
        }
        [TestMethod]
        public void TestSaveChangesWhenSaveChangesMethodOfContextRaisesNoErrorTrueShouldReturnTrue()
        {
            Assert.IsTrue(service.SaveChanges());
            contextMock.Verify(c => c.SaveChanges(), Times.Once());
        }

        
        [TestMethod]
        public void TestStrictestDeadlineAtWhenOrganisationDoesNotExistShouldReturnNull()
        {
            
			MockOrganisations( CreateExampleListOfOrganisations());
			MockEvents( CreateExampleListOfEvents());
			
            var actualDeadline = service.StrictestDeadlineAt(nonexistentOrganisationIdInTestExamples);

            Assert.AreEqual(null, actualDeadline);
        }
		[TestMethod]
		public void TestStrictestDeadlineAtWhenOrganisationHasNoEventsShouldReturnNull()
		{
			int exampleID = 1;
			SetOrganisationWithoutEventsExample();

			var actualDeadline = service.StrictestDeadlineAt(exampleID);

			Assert.AreEqual(null, actualDeadline);
		}
		[TestMethod]
		public void TestStrictestDeadlineAtWhenOrganisationHasSeveralEventsShouldReturnStrictestDeadlineAtTheGivenOrganisation()
		{
			int organisationID = 1;
			MockOrganisations(CreateExampleListOfOrganisations());
			MockEvents(CreateExampleListOfEvents());

			var expectedDeadline = new DateTime(2021, 03, 28);
			var actualDeadline = service.StrictestDeadlineAt(organisationID);

			Assert.AreEqual(expectedDeadline, actualDeadline);
		}


		[TestMethod]
		public void TestGetAvailableEventsOfWhenUserDoesNotExistShouldReturnEmptyList()
        {
			EmptyUsers();
			int nonexistentUser = 1;

			var result = service.GetAvailableEventsOf(nonexistentUser);

			Assert.IsInstanceOfType(result, typeof(IEnumerable<EventViewModel>));
			Assert.AreEqual (0, result.Count());
        }
		[TestMethod]
		public void TestGetAvailableEventsOfWhenUserHasOrganisationWithoutEventsShouldReturnEmptyList()
		{
			MockUsers(CreateExampleListOfUsers());
			SetOrganisationWithoutEventsExample();
			int userID = 1;
			var memberships = new List<Membership>
			{
				new Membership
				{
					UserId = userID,
					MemberId = 1
				}
			};
			MockMemberships(memberships);

			var result = service.GetAvailableEventsOf(userID);

			Assert.IsInstanceOfType(result, typeof(IEnumerable<EventViewModel>));
			Assert.AreEqual(0, result.Count());
		}
		[TestMethod]
		public void TestGetAvailableEventsOfWhenSomeOfTheEventsAreAvailableShouldReturnOnlyAvailableOnes()
        {
			MockOrganisations(CreateExampleListOfOrganisations());
			MockEvents(CreateExampleListOfEvents());
			MockMembers(CreateExampleListOfMembers());
			MockMemberships(CreateExampleListOfMemberships());

			var actualList = service.GetAvailableEventsOf(1);

			var expectedList = new List<EventViewModel> {
				new EventViewModel
				{
					OrganisationName = "Lorem",
					Name = "Elsõ esemény",
					DeadlineForApplication = new DateTime(2021, 03, 28),
					StartDate = new DateTime(2021, 03, 30),
					EndDate = new DateTime(2021, 04, 2),
					EventId = 1,
					Description = null,
					State = EventViewModel.EventState.Unseen
				},
				new EventViewModel
				{
					OrganisationName = "Lorem",
					Name = "Második esemény",
					DeadlineForApplication = new DateTime(2021, 04, 28),
					StartDate = new DateTime(2021, 04, 30),
					EndDate = new DateTime(2021, 05, 2),
					EventId = 2,
					Description = null,
					State = EventViewModel.EventState.Unseen
				},
				new EventViewModel
				{
					OrganisationName = "Lorem",
					Name = "Harmadik esemény",
					DeadlineForApplication = new DateTime(2021, 03, 28),
					StartDate = new DateTime(2021, 03, 30),
					EndDate = new DateTime(2021, 04, 2),
					EventId = 3,
					Description = null,
					State = EventViewModel.EventState.Unseen
				}
			};

			Assert.IsTrue(expectedList.SequenceEqual(actualList));
		}


		[TestMethod]
		public void TestCreateJobWhenOrganisationDoesNotExistShouldReturnFalse()
        {
			MockOrganisations(CreateExampleListOfOrganisations());
			MockJobs(CreateExampleListOfJobs());
			
			bool hasJobBeenCreated = service.CreateJob("Example job", nonexistentOrganisationIdInTestExamples);

			Assert.IsFalse(hasJobBeenCreated);
        }
		[TestMethod]
		public void TestCreateJobWhenOrganisationExistButGivenJobIsAlreadyRegisteredShouldReturnTrueWithoutSideEffects()
		{
			MockOrganisations(CreateExampleListOfOrganisations());
			var originalJobs = CreateExampleListOfJobs(); 
			var actualJobs = new List<Job>(originalJobs); // list is passed by reference => a copy of the original state needs to be taken for the comparison
			MockJobs(actualJobs);
			int existingOrganisationID = 1;
			String existingJob = "Junior Programmer";

			bool hasJobBeenCreated = service.CreateJob(existingJob, existingOrganisationID);

			Assert.IsTrue(hasJobBeenCreated);
			Assert.IsTrue(contextMock.Object.Jobs.SequenceEqual( originalJobs));
		}
		[TestMethod]
		public void TestCreateJobWhenOrganisationExistAndGivenJobHasNotBeenRegisteredBeforeShouldReturnTrueAndRegisterJob()
		{
			MockOrganisations(CreateExampleListOfOrganisations());
			MockJobs(CreateExampleListOfJobs());
			int existingOrganisationID = 1;
			int originalNumberOfJobs = contextMock.Object.Jobs.Count();
			int expectedNumberOfJobs = originalNumberOfJobs + 1;
			String newJob = "nonexistent";
			Job expectedNewJob = new Job
			{
				Title = newJob,
				OrganisationId = existingOrganisationID
			};

			bool hasJobBeenCreated = service.CreateJob(newJob, existingOrganisationID);

			Assert.IsTrue (hasJobBeenCreated);
			Assert.AreEqual (expectedNumberOfJobs, contextMock.Object.Jobs.Count());
			Assert.AreEqual (expectedNewJob, contextMock.Object.Jobs.Last());
		}



		[TestMethod]
		public void TestDeleteMembershipWhenUserDoesNotExistShouldReturnFalse()
		{
			MockUsers(CreateExampleListOfUsers());
			MockOrganisations(CreateExampleListOfOrganisations());
			MockMembers(CreateExampleListOfMembers());
			var memberships = CreateExampleListOfMemberships();
			MockMemberships(memberships);
			int originalNumberOfMemberships = memberships.Count();
			int invalidUserID = 100;
			int validOrganisationID = 1;

			bool membershipRemoved = service.DeleteMembership(invalidUserID, validOrganisationID);
			int newNumberOfMemberships = contextMock.Object.Memberships.Count();

			Assert.IsFalse(membershipRemoved);
			Assert.AreEqual(originalNumberOfMemberships, newNumberOfMemberships);
		}

		[TestMethod]
		public void TestDeleteMembershipWhenOrganisationDoesNotExistShouldReturnFalse()
		{
			MockUsers(CreateExampleListOfUsers());
			MockOrganisations(CreateExampleListOfOrganisations());
			MockMembers(CreateExampleListOfMembers());
			var memberships = CreateExampleListOfMemberships();
			MockMemberships(memberships);
			int originalNumberOfMemberships = memberships.Count();
			int invalidOrganisationID = 100;
			int validUserID = 1;

			bool membershipRemoved = service.DeleteMembership(validUserID, invalidOrganisationID);
			int newNumberOfMemberships = contextMock.Object.Memberships.Count();

			Assert.IsFalse(membershipRemoved);
			Assert.AreEqual(originalNumberOfMemberships, newNumberOfMemberships);
		}

		[TestMethod]
		public void TestDeleteMembershipWhenUserIsNotAMemberOfTheGivenOrganisationShouldReturnFalse()
		{
			var users = CreateExampleListOfUsers();
			int nonmemberUserID = 100;
			users.Add(new User
			{
				Id = nonmemberUserID,
				Name = "Example Name",
				Email = "example@example.com",
				UserName = "example"
			});
			MockUsers(users);
			MockOrganisations(CreateExampleListOfOrganisations());
			MockMembers(CreateExampleListOfMembers());
			var memberships = CreateExampleListOfMemberships();
			MockMemberships(memberships);
			int originalNumberOfMemberships = memberships.Count();
			int organisationID = 1;

			bool membershipRemoved = service.DeleteMembership(nonmemberUserID, organisationID);
			int newNumberOfMemberships = contextMock.Object.Memberships.Count();

			Assert.IsFalse(membershipRemoved);
			Assert.AreEqual(originalNumberOfMemberships, newNumberOfMemberships);
		}
		[TestMethod]
		public void TestDeleteMembershipWhenUserIsAMemberOfTheGivenOrganisationShouldReturnTrueAndDeleteOnlyThatMembership()
		{
			int memberUserID = 1;
			var memberships = CreateExampleListOfMemberships();
			int originalNumberOfMemberships = memberships.Count();
			int organisationID = 1;
			MockUsers(CreateExampleListOfUsers());
			MockOrganisations(CreateExampleListOfOrganisations());
			MockMembers(CreateExampleListOfMembers());
			MockMemberships(memberships);

			bool membershipRemoved = service.DeleteMembership(memberUserID, organisationID);
			int newNumberOfMemberships = contextMock.Object.Memberships.Count();

			Assert.IsTrue(membershipRemoved);
			Assert.AreEqual(originalNumberOfMemberships - 1, newNumberOfMemberships);
		}

		#endregion


		#region METHODS FOR TESTCASE SETUP
		public Mock<UserManager<User>> MockUserManager()
		{
			var storeMock = new Mock<IUserStore<User>>().Object;
			var options = new Mock<IOptions<IdentityOptions>>();
			var idOptions = new IdentityOptions();
			idOptions.Lockout.AllowedForNewUsers = false;
			options.Setup(o => o.Value).Returns(idOptions);
			var userValidators = new List<IUserValidator<User>>();
			var validator = new Mock<IUserValidator<User>>();
			userValidators.Add(validator.Object);
			var pwdValidators = new List<PasswordValidator<User>>();
			pwdValidators.Add(new PasswordValidator<User>());
			var userManager = new Mock<UserManager<User>>(storeMock, options.Object, new PasswordHasher<User>(),
				userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), null,
				new Mock<ILogger<UserManager<User>>>().Object);
			validator.Setup(v => v.ValidateAsync(userManager.Object, It.IsAny<User>()))
				.Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

			return userManager;
		}


		[TestInitialize]
		public void InitializeTest()
		{
			contextMock = new Mock<MeetingApplicationContext>();
			userManagerMock = MockUserManager();

			EmptyOrganisations();
			EmptyUsers();
			EmptyMembers();
			EmptyEvents();
			EmptyEventForms();
			EmptyJobs();

			service = new ServiceForMembers(contextMock.Object, userManagerMock.Object);
		}
		private void SetOrganisationWithoutEventsExample()
        {
			var exampleOrganisations = new List<Organisation>
			{
				new Organisation
				{
					Id = 1,
					Name = "Valami"
				}
			};
			MockOrganisations(exampleOrganisations);
			EmptyEvents();
		}

		private List<Organisation> CreateExampleListOfOrganisations()
		{
			var orgs = new List<Organisation>
			{
				new Organisation {
					Name = "Lorem",
					TypeOfStructure = TypeOfStructure.Hierarchical,
					Description = "Lorem az valami cég.",
					PermitNewMembers = true,
					Address = "Egyik utca 2.",
					AdminId = 1,
					Id = 1
				},
				new Organisation {
					Name = "Ipsum",
					TypeOfStructure = TypeOfStructure.Hierarchical,
					Description = "Az Ipsum az valami másik cég.",
					PermitNewMembers = false,
					Address = "Másik utca 7.",
					AdminId = 2,
					Id = 2
				}
			};
			return orgs;
		}
		private void MockOrganisations(List<Organisation> orgs)
		{ 
			IQueryable<Organisation> queryableOrganisationData = orgs.AsQueryable();
			var organisationMock = new Mock<DbSet<Organisation>>();
			organisationMock.As<IQueryable<Organisation>>().Setup(mock => mock.ElementType).Returns(queryableOrganisationData.ElementType);
			organisationMock.As<IQueryable<Organisation>>().Setup(mock => mock.Expression).Returns(queryableOrganisationData.Expression);
			organisationMock.As<IQueryable<Organisation>>().Setup(mock => mock.Provider).Returns(queryableOrganisationData.Provider);
			organisationMock.As<IQueryable<Organisation>>().Setup(mock => mock.GetEnumerator()).Returns(orgs.GetEnumerator());

			contextMock.Setup<DbSet<Organisation>>(c => c.Organisations).Returns(organisationMock.Object);
		}
		private void EmptyOrganisations()
		{
			MockOrganisations(new List<Organisation>());
		}
		private List<Event> CreateExampleListOfEvents()
		{
			var organisationIds = contextMock.Object.Organisations.Select(o => o.Id).ToList();

			var events = new List<Event>
			{
				new Event
				{
					OrganisationId = organisationIds[0],
					Name = "Elsõ esemény",
					DeadlineForApplication = new DateTime(2021, 03, 28),
					StartDate = new DateTime(2021, 03, 30),
					EndDate = new DateTime(2021, 04, 2),
					Id = 1
				},
				new Event
				{
					OrganisationId = organisationIds[0],
					Name = "Második esemény",
					DeadlineForApplication = new DateTime(2021, 04, 28),
					StartDate = new DateTime(2021, 04, 30),
					EndDate = new DateTime(2021, 05, 2),
					Id = 2
				},
				new Event
				{
					OrganisationId = organisationIds[0],
					Name = "Harmadik esemény",
					DeadlineForApplication = new DateTime(2021, 03, 28),
					StartDate = new DateTime(2021, 03, 30),
					EndDate = new DateTime(2021, 04, 2),
					Id = 3
				},
				new Event
				{
					OrganisationId = organisationIds[1],
					Name = "Negyedik esemény",
					DeadlineForApplication = new DateTime(2021, 02, 28),
					StartDate = new DateTime(2021, 04, 30),
					EndDate = new DateTime(2021, 05, 2),
					Id = 4
				},
				new Event
				{
					OrganisationId = organisationIds[1],
					Name = "Ötödik esemény",
					DeadlineForApplication = new DateTime(2021, 06, 28),
					StartDate = new DateTime(2021, 03, 30),
					EndDate = new DateTime(2021, 04, 2),
					Id = 5
				}
			};

			return events;
		}
		private void MockEvents(List<Event> events) 
		{
			IQueryable<Event> queryableEventData = events.AsQueryable();
			var eventMock = new Mock<DbSet<Event>>();
			eventMock.As<IQueryable<Event>>().Setup(mock => mock.ElementType).Returns(queryableEventData.ElementType);
			eventMock.As<IQueryable<Event>>().Setup(mock => mock.Expression).Returns(queryableEventData.Expression);
			eventMock.As<IQueryable<Event>>().Setup(mock => mock.Provider).Returns(queryableEventData.Provider);
			eventMock.As<IQueryable<Event>>().Setup(mock => mock.GetEnumerator()).Returns(events.GetEnumerator()); // a korábban megadott listát fogjuk visszaadni

			contextMock.Setup<DbSet<Event>>(c => c.Events).Returns(eventMock.Object);
		}
		private void EmptyEvents()
		{
			MockEvents(new List<Event>());
		}
		private void MockEventForms(List<EventForm> events)
		{
			IQueryable<EventForm> queryableEventData = events.AsQueryable();
			var eventMock = new Mock<DbSet<EventForm>>();
			eventMock.As<IQueryable<EventForm>>().Setup(mock => mock.ElementType).Returns(queryableEventData.ElementType);
			eventMock.As<IQueryable<EventForm>>().Setup(mock => mock.Expression).Returns(queryableEventData.Expression);
			eventMock.As<IQueryable<EventForm>>().Setup(mock => mock.Provider).Returns(queryableEventData.Provider);
			eventMock.As<IQueryable<EventForm>>().Setup(mock => mock.GetEnumerator()).Returns(events.GetEnumerator()); // a korábban megadott listát fogjuk visszaadni

			contextMock.Setup<DbSet<EventForm>>(c => c.EventForms).Returns(eventMock.Object);
		}
		private void EmptyEventForms()
		{
			MockEventForms(new List<EventForm>());
		}
		private List<Member> CreateExampleListOfMembers()
		{
			MockOrganisations(CreateExampleListOfOrganisations());
			var organisationIds = contextMock.Object.Organisations.Select(o => o.Id).ToList();
			var members = new List<Member>
			{
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "akarki@gmail.com",
					Name = "Nem Tomi",
					Id = 1
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "akark@gmail.com",
					Name = "Nem Tom",
					Id = 2
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "akar@gmail.com",
					Name = "Nem To",
					Id = 3
				}
			};
			return members;
		}
		private void MockMembers(List<Member> members) 
		{ 
			IQueryable<Member> queryableMemberData = members.AsQueryable();
			var memberMock = new Mock<DbSet<Member>>();
			memberMock.As<IQueryable<Member>>().Setup(mock => mock.ElementType).Returns(queryableMemberData.ElementType);
			memberMock.As<IQueryable<Member>>().Setup(mock => mock.Expression).Returns(queryableMemberData.Expression);
			memberMock.As<IQueryable<Member>>().Setup(mock => mock.Provider).Returns(queryableMemberData.Provider);
			memberMock.As<IQueryable<Member>>().Setup(mock => mock.GetEnumerator()).Returns(members.GetEnumerator()); // a korábban megadott listát fogjuk visszaadni

			contextMock.Setup<DbSet<Member>>(c => c.Members).Returns(memberMock.Object);
		}
		private void EmptyMembers()
        {
			MockMembers(new List<Member>());
		}
		private List<User> CreateExampleListOfUsers()
		{
			var users = new List<User>{
				new User
				{
					UserName = "admin",
					Name = "Nem Tomi",
					Email = "admin@example.com",
					PhoneNumber = "+36123456789",
					Address = "Nevesincs utca 1.",
					Id = 1
				},
				new User
				{
					UserName = "user",
					Name = "Valami User",
					Email = "user@example.com",
					PhoneNumber = "+36123456789",
					Address = "User utca 1.",
					Id = 2
				}
			};
			return users;
		}
		private List<Membership> CreateExampleListOfMemberships()
		{
			var memberships = new List<Membership>
			{
				new Membership
				{
					UserId = 1,
					MemberId = 1
				},
				new Membership
				{
					UserId = 1,
					MemberId = 2
				}
			};
			return memberships;
		}
		private void MockMemberships(List<Membership> memberships)
		{
			IQueryable<Membership> queryableMembershipData = memberships.AsQueryable();
			var memberMock = new Mock<DbSet<Membership>>();
			memberMock.As<IQueryable<Membership>>().Setup(mock => mock.ElementType).Returns(queryableMembershipData.ElementType);
			memberMock.As<IQueryable<Membership>>().Setup(mock => mock.Expression).Returns(queryableMembershipData.Expression);
			memberMock.As<IQueryable<Membership>>().Setup(mock => mock.Provider).Returns(queryableMembershipData.Provider);
			memberMock.As<IQueryable<Membership>>().Setup(mock => mock.GetEnumerator()).Returns(memberships.GetEnumerator()); // a korábban megadott listát fogjuk visszaadni

			memberMock.Setup(mock => mock.Remove(It.IsAny<Membership>())).Callback<Membership>(m =>
			{
				memberships.Remove(m);
			});

			contextMock.Setup<DbSet<Membership>>(c => c.Memberships).Returns(memberMock.Object);
		}
		private void EmptyMemberships()
		{
			MockMemberships(new List<Membership>());
		}
		private void MockUsers(List<User> users) 
		{ 
			IQueryable<User> queryableUserData = users.AsQueryable();
			var userMock = new Mock<DbSet<User>>();
			userMock.As<IQueryable<User>>().Setup(mock => mock.ElementType).Returns(queryableUserData.ElementType);
			userMock.As<IQueryable<User>>().Setup(mock => mock.Expression).Returns(queryableUserData.Expression);
			userMock.As<IQueryable<User>>().Setup(mock => mock.Provider).Returns(queryableUserData.Provider);
			userMock.As<IQueryable<User>>().Setup(mock => mock.GetEnumerator()).Returns(users.GetEnumerator()); // a korábban megadott listát fogjuk visszaadni

			userManagerMock.Setup<IQueryable<User>>(c => c.Users).Returns(userMock.Object);
			contextMock.Setup<IQueryable<User>>(c => c.Users).Returns(userMock.Object);
		}
		private void EmptyUsers()
        {
			MockUsers(new List<User>());
		}

		
		private List<Job> CreateExampleListOfJobs()
		{
			var organisationIds = contextMock.Object.Organisations.Select(o => o.Id).ToList();

			var jobs = new List<Job>
			{
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Junior Programmer",
					Weight = 5
				},
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Senior Programmer",
					Weight = 10
				},
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Intern Programmer",
					Weight = 1
				},
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Manager",
					Weight = 5
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Junior Data Analyst",
					Weight = 5
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Senior Data Analyst",
					Weight = 10
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Intern Data Analyst",
					Weight = 1
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Manager",
					Weight = 5
				}
			};
			
			return jobs;
		}
		private void MockJobs(List<Job> jobs)
		{
			IQueryable<Job> queryableJobData = jobs.AsQueryable();
			var jobMock = new Mock<DbSet<Job>>();
			jobMock.As<IQueryable<Job>>().Setup(mock => mock.ElementType).Returns(queryableJobData.ElementType);
			jobMock.As<IQueryable<Job>>().Setup(mock => mock.Expression).Returns(queryableJobData.Expression);
			jobMock.As<IQueryable<Job>>().Setup(mock => mock.Provider).Returns(queryableJobData.Provider);
			jobMock.As<IQueryable<Job>>().Setup(mock => mock.GetEnumerator()).Returns(jobs.GetEnumerator()); // a korábban megadott listát fogjuk visszaadni

			jobMock.Setup(mock => mock.Add(It.IsAny<Job>())).Callback<Job>(j =>
			{
				jobs.Add(j);
			});

			contextMock.Setup<DbSet<Job>>(c => c.Jobs).Returns(jobMock.Object);
		}
		private void EmptyJobs()
		{
			MockJobs(new List<Job>());
		}
        /*
		private void CreateExampleListOfProjects()
		{
			var organisationIds = contextMock.Object.Organisations.Select(o => o.Id).ToList();

			Project[] projects = new Project[]
			{
				new Project
				{
					Name = "Elsõ projekt",
					OrganisationId = organisationIds[0],
					Weight = 10
				},
				new Project
				{
					Name = "Második projekt",
					OrganisationId = organisationIds[0],
					Weight = 10
				},
				new Project
				{
					Name = "Harmadik projekt",
					OrganisationId = organisationIds[0],
					Weight = 10
				},
				new Project
				{
					Name = "Negyedik projekt",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Ötödik projekt",
					OrganisationId = organisationIds[1],
					Weight = 10
				}
			};

			foreach (var p in projects)
			{
				contextMock.Object.Projects.Add(p);
			}
		}
		private void CreateExampleListOfAcceptedEmailDomains()
		{
			var organisationIds = contextMock.Object.Organisations.Select(o => o.Id).ToList();

			var domains = new AcceptedEmailDomain[]
			{
				new AcceptedEmailDomain {
					DomainName = "inf.elte.hu",
					OrganisationId = organisationIds[0]
				},
				new AcceptedEmailDomain {
					DomainName = "caesar.elte.hu",
					OrganisationId = organisationIds[0]
				}
			};
			foreach (var d in domains)
			{
				contextMock.Object.AcceptedEmailDomains.Add(d);
			}

			contextMock.Object.SaveChanges();
		}
		private void CreateExampleListOfVenues()
		{
			var eventIds = contextMock.Object.Events.Select(o => o.Id).ToList();

			contextMock.Object.Venues.Add(
				new Venue
				{
					EventId = eventIds[0],
					Description = "Elsõ mondat. Második mondat.",
					Address = "1000 Budapest, Valami utca 10.",
					LocationX = 45.4591,
					LocationY = 12.5068,
					GuestLimit = 30,
					Name = "Elsõ helyszín"
				});
			contextMock.Object.Venues.Add(
				new Venue
				{
					EventId = eventIds[0],
					Description = "Elsõ mondat. Második mondat.",
					Address = "1000 Budapest, Második utca 10.",
					LocationX = 45.4591,
					LocationY = 10.5068,
					GuestLimit = 30,
					Name = "Második helyszín"
				});
			contextMock.Object.Venues.Add(
				new Venue
				{
					EventId = eventIds[2],
					Description = "Elsõ mondat. Második mondat.",
					Address = "1000 Budapest, Harmadik utca 10.",
					LocationX = 46.4591,
					LocationY = 11.5068,
					GuestLimit = 30,
					Name = "Harmadik helyszín"
				});

			contextMock.Object.SaveChanges();
		}
		private void CreateExampleListOfVenueImages(string imageDirectory)
		{
			var venueIds = contextMock.Object.Venues.Select(o => o.Id).ToList();

			// Ellenõrizzük, hogy képek könyvtára létezik-e.
			if (Directory.Exists(imageDirectory))
			{
				var images = new List<VenueImage>();

				// Képek aszinkron betöltése.
				var largePath = Path.Combine(imageDirectory, "petra_1.png");
				var smallPath = Path.Combine(imageDirectory, "petra_1_thumb.png");
				if (File.Exists(largePath) && File.Exists(smallPath))
				{
					images.Add(new VenueImage
					{
						VenueId = venueIds[0],
						ImageLarge = File.ReadAllBytes(largePath),
						ImageSmall = File.ReadAllBytes(smallPath)
					});
				}

				largePath = Path.Combine(imageDirectory, "petra_2.png");
				smallPath = Path.Combine(imageDirectory, "petra_2_thumb.png");
				if (File.Exists(largePath) && File.Exists(smallPath))
				{
					images.Add(new VenueImage
					{
						VenueId = venueIds[0],
						ImageLarge = File.ReadAllBytes(largePath),
						ImageSmall = File.ReadAllBytes(smallPath)
					});
				}

				largePath = Path.Combine(imageDirectory, "cavallino_1.png");
				smallPath = Path.Combine(imageDirectory, "cavallino_1_thumb.png");
				if (File.Exists(largePath) && File.Exists(smallPath))
				{
					images.Add(new VenueImage
					{
						VenueId = venueIds[2],
						ImageLarge = File.ReadAllBytes(largePath),
						ImageSmall = File.ReadAllBytes(smallPath)
					});
				}

				foreach (var image in images)
				{
					contextMock.Object.VenueImages.Add(image);
				}

				contextMock.Object.SaveChanges();
			}
		}
		*/
        #endregion
    }
}