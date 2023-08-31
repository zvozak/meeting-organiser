using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonData.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommonData
{
	public class DbInitializer
	{
		static Random randomNumberGenerator = new Random();
		static readonly DateTime earliestJoiningDate = new DateTime(1990, 1, 1);
		static readonly int rangeOfJoiningDates = (DateTime.Today - earliestJoiningDate).Days;

		private static MeetingApplicationContext context;
		private static UserManager<User> userManager;
		private static RoleManager<IdentityRole<int>> roleManager;

		public static void Initialize(IServiceProvider serviceProvider, string imageDirectory)
		{
			context = serviceProvider.GetRequiredService<MeetingApplicationContext>();
			userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            try
            {
				context.Database.Migrate();
            }
            catch
            {

            }


			if (!context.Users.Any())
            {
				SeedUsers();
            }
			if (!context.Organisations.Any())
			{
				SeedOrganisations();
				SeedEvents();
				SeedVenues();
				SeedJobs();
			}
            if (!context.Jobs.Any())
            {
				SeedJobs();
            }
            if (!context.Members.Any())
            {
				SeedMembers();
				SeedMemberships();
			}
            if (!context.AcceptedEmailDomains.Any())
            {
				SeedAcceptedEmailDomains();
            }
            if (!context.Projects.Any())
            {
				SeedProjects();
				SeedMemberOfProjects();
            }
			if (!context.MemberOfProjects.Any())
			{
				SeedMemberOfProjects();
			}
			if (!context.Venues.Any())
            {
				SeedVenues();
				SeedVenueImages(imageDirectory);
            }
            if (!context.VenueImages.Any())
            {
				SeedVenueImages(imageDirectory);
            }
            if (!context.Events.Any())
            {
				SeedEvents();
			}
		}
		private static void SeedJobs()
        {
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();

			Job[] jobs = new Job[]
			{
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Junior Software Engineer",
					Weight = 2
				},
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Senior Software Engineer",
					Weight = 4
				},
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Software Engineer Intern",
					Weight = 1
				},
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Engineering Manager",
					Weight = 6
				},
				new Job
				{
					OrganisationId = organisationIds[0],
					Title = "Senior Engineering Manager",
					Weight = 9
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Junior Software Engineer",
					Weight = 2
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Senior Software Engineer",
					Weight = 4
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Software Engineer Intern",
					Weight = 1
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Senior Data Analyst",
					Weight = 4
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Junior Data Analyst",
					Weight = 1
				},
				new Job
				{
					OrganisationId = organisationIds[1],
					Title = "Scrum master",
					Weight = 9
				}
			};
			foreach (var j in jobs)
			{
				context.Jobs.Add(j);
				context.SaveChanges();
			}
		}
		private static void SeedProjects()
		{
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();

			Project[] projects = new Project[]
			{
				new Project
				{
					Name = "CI/CD",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Security",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Infrastructure",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Testing",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Frontend",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Automation",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Graphics",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Design",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Critical Systems",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "API",
					OrganisationId = organisationIds[1],
					Weight = 10
				},
				new Project
				{
					Name = "Support",
					OrganisationId = organisationIds[1],
					Weight = 10
				}
			};

			foreach(var p in projects)
            {
				context.Projects.Add(p);
				context.SaveChanges();
            }
		}
		private static void SeedMemberOfProjects()
        {
			var projectMemberships = new MemberOfProject[]
			{
				new MemberOfProject
				{
					MemberId = 13,
					ProjectId = 1
				},
				new MemberOfProject
				{
					MemberId = 14,
					ProjectId = 1
				},
				new MemberOfProject
				{
					MemberId = 15,
					ProjectId = 1
				},
				new MemberOfProject
				{
					MemberId = 13,
					ProjectId = 2
				},
				new MemberOfProject
				{
					MemberId = 25,
					ProjectId = 2
				},
				new MemberOfProject
				{
					MemberId = 16,
					ProjectId = 3
				},
				new MemberOfProject
				{
					MemberId = 17,
					ProjectId = 3
				},
				new MemberOfProject
				{
					MemberId = 18,
					ProjectId = 3
				},
				new MemberOfProject
				{
					MemberId = 19,
					ProjectId = 3
				},
				new MemberOfProject
				{
					MemberId = 19,
					ProjectId = 4
				},
				new MemberOfProject
				{
					MemberId = 20,
					ProjectId = 4
				},
				new MemberOfProject
				{
					MemberId = 20,
					ProjectId = 5
				},
				new MemberOfProject
				{
					MemberId = 21,
					ProjectId = 5
				},
				new MemberOfProject
				{
					MemberId = 21,
					ProjectId = 6
				},
				new MemberOfProject
				{
					MemberId = 22,
					ProjectId = 6
				},
				new MemberOfProject
				{
					MemberId = 21,
					ProjectId = 7
				},
				new MemberOfProject
				{
					MemberId = 23,
					ProjectId = 7
				},
				new MemberOfProject
				{
					MemberId = 24,
					ProjectId = 7
				},
				new MemberOfProject
				{
					MemberId = 25,
					ProjectId = 8
				},
				new MemberOfProject
				{
					MemberId = 24,
					ProjectId = 8
				},
				new MemberOfProject
				{
					MemberId = 26,
					ProjectId = 9
				},
				new MemberOfProject
				{
					MemberId = 19,
					ProjectId = 9
				},
				new MemberOfProject
				{
					MemberId = 26,
					ProjectId = 10
				},
				new MemberOfProject
				{
					MemberId = 16,
					ProjectId = 10
				},
				new MemberOfProject
				{
					MemberId = 20,
					ProjectId = 10
				}
			};

			foreach (var membership in projectMemberships)
			{
				context.MemberOfProjects.Add(membership);
			}

			context.SaveChanges();
		}
		private static void SeedAcceptedEmailDomains()
        {
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();

			var domains = new AcceptedEmailDomain[]
			{
				new AcceptedEmailDomain {
					DomainName = "apple.com",
					OrganisationId = organisationIds[1]
				}
			};
			foreach (var d in domains)
			{
				context.AcceptedEmailDomains.Add(d);
			}

			context.SaveChanges();
		}
		private static void SeedOrganisations()
		{
			var orgs = new Organisation[]
			{
				new Organisation {
					Name = "Apple Inc.",
					TypeOfStructure = TypeOfStructure.Hierarchical,
					Description = "Apple Inc. is an American multinational technology company that specializes in consumer electronics, computer software and online services. ",
					PermitNewMembers = true,
					Address = "Budapest, Bajcsy-Zsilinszky út 78, 1055",
					AdminId = context.Users.First(u => u.UserName == "appleAdmin").Id
				},
				new Organisation {
					Name = "Microsoft Corporation",
					TypeOfStructure = TypeOfStructure.ProjectBased,
					Description = "Microsoft Corporation is an American multinational technology corporation which produces computer software, consumer electronics, personal computers, and related services",
					PermitNewMembers = false,
					Address = "Budapest, M épület, Graphisoft park 3, 1031",
					AdminId = context.Users.First(u => u.UserName == "microsoftAdmin").Id
				}
			};
			foreach (var o in orgs)
			{
				context.Organisations.Add(o);
				context.SaveChanges();
			}
		}

		private static void SeedEvents()
		{
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();

			var events = new Event[]{
				new Event {
					OrganisationId = organisationIds[0],
					Name = "Python Programming Course",
					DeadlineForApplication = new DateTime(2022, 03, 28),
					StartDate = new DateTime(2022, 03, 30,8,0,0),
					EndDate = new DateTime(2022, 03, 30, 16, 0, 0),
					Description = "This class explores advanced Python topics and skills with a focus on enterprise development. You’ll learn how to leverage OS services, code graphical application interfaces, create modules and run unit tests, define classes, interact with network series, query databases, and processes XML data. This comprehensive course provides an in-depth exploration of working with the programming language for enterprise development. At the conclusion, you will be able to use Python to complete advanced tasks in the real world."
				},
				new Event {
					OrganisationId = organisationIds[0],
					Name = "Agile Training",
					DeadlineForApplication = new DateTime(2022, 04, 28),
					StartDate = new DateTime(2022, 04, 30, 8, 0, 0),
					EndDate = new DateTime(2022, 05, 2, 11, 0, 0),
					Description = "You want to be able to deliver value frequently, learn from feedback, and then deliver changes that delight customers. Start learning how with this training."
				},
				new Event {
					OrganisationId = organisationIds[0],
					Name = "Year End Company Retrospective",
					DeadlineForApplication = new DateTime(2022, 03, 28),
					StartDate = new DateTime(2022, 03, 30,10,0,0),
					EndDate = new DateTime(2022, 03, 30, 15, 30, 0),
					Description = "Reflect on workflows and methods of development."
				},
				new Event {
					OrganisationId = organisationIds[1],
					Name = "Design Patterns Course",
					DeadlineForApplication = new DateTime(2022, 04, 28),
					StartDate = new DateTime(2022, 04, 30),
					EndDate = new DateTime(2022, 05, 2),
					Description = "The Design Patterns Library contains descriptions and examples of software design patterns that you can apply in your daily development. These patterns are time proven techniques for building long-lived, well factored software that are widely used in software development today."
				},
				new Event {
					OrganisationId = organisationIds[1],
					Name = "Q&A Session with Partners",
					DeadlineForApplication = new DateTime(2022, 03, 28),
					StartDate = new DateTime(2022, 03, 30, 14, 0, 0),
					EndDate = new DateTime(2022, 04, 2, 15, 30, 0),
					Description = "We will show the new release to our partners, and answer arising questions."
				},
				new Event {
					OrganisationId = organisationIds[1],
					Name = "Python Programming Course",
					DeadlineForApplication = new DateTime(2022, 03, 28),
					StartDate = new DateTime(2022, 03, 30,8,0,0),
					EndDate = new DateTime(2022, 03, 30, 16, 0, 0),
					Description = "This class explores advanced Python topics and skills with a focus on enterprise development. You’ll learn how to leverage OS services, code graphical application interfaces, create modules and run unit tests, define classes, interact with network series, query databases, and processes XML data. This comprehensive course provides an in-depth exploration of working with the programming language for enterprise development. At the conclusion, you will be able to use Python to complete advanced tasks in the real world."
				},
				new Event {
					OrganisationId = organisationIds[1],
					Name = "Agile Training",
					DeadlineForApplication = new DateTime(2022, 04, 28),
					StartDate = new DateTime(2022, 04, 30, 8, 0, 0),
					EndDate = new DateTime(2022, 05, 2, 11, 0, 0),
					Description = "You want to be able to deliver value frequently, learn from feedback, and then deliver changes that delight customers. Start learning how with this training."
				},
				new Event {
					OrganisationId = organisationIds[1],
					Name = "Year End Company Retrospective",
					DeadlineForApplication = new DateTime(2022, 03, 28),
					StartDate = new DateTime(2022, 03, 30,10,0,0),
					EndDate = new DateTime(2022, 03, 30, 15, 30, 0),
					Description = "Reflect on workflows and methods of development."
				},
			};

            foreach (var e in events)
            {
				context.Events.Add(e);
				context.SaveChanges();
            }
		}

		private static void SeedMembers()
		{
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();
			var members = new Member[]
			{
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "samanda@apple.com",
					Name = "Amanda Smith",
					JobId = 5,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "dbrandon@apple.com",
					Name = "Brandon Daniels",
					JobId = 2,
					BossId = 1,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "lchristina@apple.com",
					Name = "Christina Lambert",
					JobId = 4,
					BossId = 1,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "hdavid@apple.com",
					Name = "David Hunt",
					JobId = 4,
					BossId = 1,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "keric@apple.com",
					Name = "Eric Kimberley",
					JobId = 1,
					BossId = 2,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "cfrank@apple.com",
					Name = "Frank Castle",
					JobId = 1,
					BossId = 2,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "ogary@apple.com",
					Name = "Gary Oldman",
					JobId = 2,
					BossId = 3,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "gheather@apple.com",
					Name = "Heather Garcia",
					JobId = 2,
					BossId = 4,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "jian@apple.com",
					Name = "Ian Jones",
					JobId = 1,
					BossId = 7,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "bkeric@apple.com",
					Name = "Jeffery Brown",
					JobId = 1,
					BossId = 7,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "wkeric@apple.com",
					Name = "Karen Williams",
					JobId = 1,
					BossId = 7,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "jkeric@apple.com",
					Name = "Larry Johnson",
					JobId = 3,
					BossId = 7,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "samanda@microsoft.com",
					Name = "Amanda Smith",
					JobId = 6,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "dbrandon@microsoft.com",
					Name = "Brandon Daniels",
					JobId = 7,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "lchristina@microsoft.com",
					Name = "Christina Lambert",
					JobId = 8,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "hdavid@microsoft.com",
					Name = "David Hunt",
					JobId = 11,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "keric@microsoft.com",
					Name = "Eric Kimberley",
					JobId = 7,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "cfrank@microsoft.com",
					Name = "Frank Castle",
					JobId = 6,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "ogary@microsoft.com",
					Name = "Gary Oldman",
					JobId = 7,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "gheather@microsoft.com",
					Name = "Heather Garcia",
					JobId = 11,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "jian@microsoft.com",
					Name = "Ian Jones",
					JobId = 10,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "bjeffrey@microsoft.com",
					Name = "Jeffery Brown",
					JobId = 11,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "wkarenc@microsoft.com",
					Name = "Karen Williams",
					JobId = 6,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "jlarry@microsoft.com",
					Name = "Larry Johnson",
					JobId = 6,
					Department = "Research",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "smonica@microsoft.com",
					Name = "Monica Smith",
					JobId = 9,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "snatasha@microsoft.com",
					Name = "Natasha Smith",
					JobId = 11,
					Department = "Production",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates))
				}
			};
			foreach (var member in members)
            {
				context.Add(member);
				context.SaveChanges();
			}
		}
		private static void SeedMemberships()
        {
			var memberships = new Membership[]
			{
				new Membership
				{
					MemberId = 1,
					UserId = 2
				},
				new Membership
				{
					MemberId = 12,
					UserId = 3
				}
			};

            foreach (var membership in memberships)
            {
				context.Memberships.Add(membership);
            }

			context.SaveChanges();
        }
		private static void SeedUsers()
		{
			var adminUser = new User
			{
				UserName = "appleAdmin",
				Name = "Sarah Hills",
				Email = "appleadmin@apple.com",
				PhoneNumber = "+36123456789"
			};
			var adminPassword = "Almafa123";
			var adminRole = new IdentityRole<int>("administrator");

			var result1 = userManager.CreateAsync(adminUser, adminPassword).Result;
			var result2 = roleManager.CreateAsync(adminRole).Result;
			var result3 = userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;

			var adminUser2 = new User
			{
				UserName = "microsoftAdmin",
				Name = "Sarah Hills",
				Email = "microsoftadmin@microsoft.com",
				PhoneNumber = "+36123456789"
			};
			var adminPassword2 = "Almafa123";

			var result4 = userManager.CreateAsync(adminUser2, adminPassword2).Result;
			var result5 = userManager.AddToRoleAsync(adminUser2, adminRole.Name).Result;

			var user = new User
			{
				UserName = "samanda",
				Name = "Amanda Smith",
				Email = "samanda@apple.com",
				PhoneNumber = "+36123456789"
			};
			var userPassword = "Almafa123";
			var userRole = new IdentityRole<int>("user");

			var result6 = userManager.CreateAsync(user, userPassword).Result;
			var result7 = roleManager.CreateAsync(userRole).Result;
			var result8 = userManager.AddToRoleAsync(user, userRole.Name).Result;

			var user2 = new User
			{
				UserName = "samanda",
				Name = "Amanda Smith",
				Email = "samanda@microsoft.com",
				PhoneNumber = "+36123456789"
			};
			var userPassword2 = "Almafa123";

			var result9 = userManager.CreateAsync(user2, userPassword2).Result;
			var result10 = userManager.AddToRoleAsync(user, userRole.Name).Result;

		}
		private static void SeedVenues()
        {
			var eventIds = context.Events.Select(o => o.Id).ToList();

			context.Venues.Add(
				new Venue
				{
					EventId = eventIds[3],
					Description = "Buffet and cloakroom available.",
					Address = "Budapest, Pázmány Péter stny. 1c, 1117",
					LocationX = 45.4591,
					LocationY = 12.5068,
					GuestLimit = 30,
					Name = "Lágymányosi ELTE Campus - Southern Block"
				});
			context.Venues.Add(
				new Venue
				{
					EventId = eventIds[3],
					Description = "",
					Address = "Budapest, Infopark stny. 1i, 1117",
					LocationX = 45.4591,
					LocationY = 10.5068,
					GuestLimit = 30,
					Name = "Budapest, Infopark stny. 1i, 1117"
				});
			context.Venues.Add(
				new Venue
				{
					EventId = eventIds[4],
					Description = "Lunch included.",
					Address = "Budapest, Petőfi híd, 1117",
					LocationX = 46.4591,
					LocationY = 11.5068,
					GuestLimit = 30,
					Name = "A38"
				});

			context.SaveChanges();
		}
		private static void SeedVenueImages(string imageDirectory)
		{
			var venueIds = context.Venues.Select(o => o.Id).ToList();

			// Ellenőrizzük, hogy képek könyvtára létezik-e.
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
					context.VenueImages.Add(image);
				}

				context.SaveChanges();
			}
		}
	}
}
