using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonData.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommonData
{
	public class DbInitializerForTwoDifferentTypeOfOrganisations
	{
		private static MeetingApplicationContext context;
		private static UserManager<User> userManager;
		private static RoleManager<IdentityRole<int>> roleManager;

		static string solutionDirectory = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName;
		static string projectsTXTpath = Path.Combine(solutionDirectory, "CommonData\\ExampleDataSource\\projects.txt");
		static string namesTXTpath = Path.Combine(solutionDirectory, "CommonData\\ExampleDataSource\\names.txt");
		static string jobsTXTpath = Path.Combine(solutionDirectory, "CommonData\\ExampleDataSource\\jobs.txt");

		static readonly string[] possibleProjects = System.IO.File.ReadAllLines(projectsTXTpath);
		static readonly string[] possibleNames = System.IO.File.ReadAllLines(namesTXTpath);
		static readonly string[] possibleJobs = System.IO.File.ReadAllLines(jobsTXTpath);

		static readonly int maxNumberOfJobs = 50;
		static readonly int maxNumberOfMembers = 200;
		static readonly int maxNumberOfProjects = 50;

		static int hierchicalOrganisationID;
		static int projectbasedOrganisationID;
		static int firstMemberInHierarchicalID;
		static int lastMemberInHierarchicalID;
		static int firstMemberInProjectBasedID;
		static int lastMemberInProjectBasedID;
		static int firstJobInHierarchicalID;
		static int lastJobInHierarchicalID;
		static int firstJobInProjectBasedID;
		static int lastJobInProjectBasedID;
		static int firstProjectID;
		static int lastProjectID;

		static Random randomNumberGenerator = new Random();
		static readonly DateTime earliestJoiningDate = new DateTime(1990, 1, 1);
		static readonly int rangeOfJoiningDates = (DateTime.Today - earliestJoiningDate).Days;

		public static void Initialize(IServiceProvider serviceProvider, string imageDirectory)
		{
			context = serviceProvider.GetRequiredService<MeetingApplicationContext>();
			userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();

			SeedUsers();
            SeedOrganisations();
			SeedEvents();
			SeedVenues();
			SeedVenueImages(imageDirectory);
			
			SeedJobsForProjectBasedOrganisation(maxNumberOfJobs);
			SeedMembersForProjectBasedOrganisation(maxNumberOfMembers);
			SeedProjectsForProjectBasedOrganisation(maxNumberOfProjects);
			SeedMemberOfProjects();
			
            SeedJobsForHierarchicalOrganisation(maxNumberOfJobs);
			SeedMembersForHierarchicalOrganisation(maxNumberOfMembers);
		}
		private static void SeedOrganisations()
		{
			var orgs = new Organisation[]
			{
				new Organisation {
					Name = "Hierarchical org",
					TypeOfStructure = TypeOfStructure.Hierarchical,
					Description = "",
					PermitNewMembers = true,
					Address = "Egyik utca 2.",
					AdminId = context.Users.Single(u => u.UserName == "adminH").Id
				},
				new Organisation {
					Name = "Project based org",
					TypeOfStructure = TypeOfStructure.ProjectBased,
					Description = "",
					PermitNewMembers = false,
					Address = "Másik utca 7.",
					AdminId = context.Users.Single(u => u.UserName == "adminP").Id
				}
			};
			foreach (var o in orgs)
			{
				context.Organisations.Add(o);
			}

			context.SaveChanges();

			hierchicalOrganisationID = context.Organisations.Single(o => o.TypeOfStructure == TypeOfStructure.Hierarchical).Id;
			projectbasedOrganisationID = context.Organisations.Single(o => o.TypeOfStructure == TypeOfStructure.ProjectBased).Id;
		}
		private static void SeedMembersForProjectBasedOrganisation(int numberOfMembers)
		{
			context.Members.Add(new Member
			{
				Name = possibleNames[0],
				Email = possibleNames[0].ToLower() + "@gmail.com",
				Department = "",
				DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates)),
				OrganisationId = projectbasedOrganisationID,
				JobId = randomNumberGenerator.Next(firstJobInProjectBasedID, lastJobInProjectBasedID)
			});
			context.SaveChanges();

			firstMemberInProjectBasedID = context.Members.First().Id;
			for (int i = 1; i < numberOfMembers; i++)
			{
				context.Members.Add(new Member
				{
					Name = possibleNames[i],
					Email = possibleNames[i].ToLower() + "@gmail.com",
					Department = "",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates)),
					OrganisationId = projectbasedOrganisationID,
					JobId = randomNumberGenerator.Next(firstJobInProjectBasedID, lastJobInProjectBasedID)
				});
				context.SaveChanges();
			}

			lastMemberInProjectBasedID = firstMemberInProjectBasedID + context.Members.Count() - 1;
		}
		private static void SeedMembersForHierarchicalOrganisation(int numberOfMembers)
		{
			firstMemberInHierarchicalID = lastMemberInProjectBasedID + 1;

			context.Members.Add(new Member
			{
				Name = possibleNames[0],
				Email = possibleNames[0].ToLower() + "@gmail.com",
				Department = "",
				DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates)),
				OrganisationId = hierchicalOrganisationID,
				JobId = randomNumberGenerator.Next(firstJobInProjectBasedID, lastJobInProjectBasedID),
				BossId = null
			});

			context.SaveChanges();

			for (int i = 1; i < numberOfMembers; i++)
			{
				var member = new Member
				{
					Name = possibleNames[i],
					Email = possibleNames[i].ToLower() + "@gmail.com",
					Department = "",
					DateOfJoining = earliestJoiningDate.AddDays(randomNumberGenerator.Next(rangeOfJoiningDates)),
					OrganisationId = hierchicalOrganisationID,
					JobId = randomNumberGenerator.Next(firstJobInHierarchicalID, lastJobInHierarchicalID),
					BossId = randomNumberGenerator.Next(firstMemberInHierarchicalID, firstMemberInHierarchicalID + i - 1)
				};

				context.Members.Add(member);
				context.SaveChanges();
			}
			
			lastMemberInHierarchicalID = firstMemberInProjectBasedID + context.Members.Count() - 1;
		}
		private static void SeedJobsForHierarchicalOrganisation(int numberOfJobs)
        {
			
			if (numberOfJobs > maxNumberOfJobs)
			{
				throw new ArgumentException("You can have at most 50 jobs.");
			}

			for (int i = 0; i < numberOfJobs; i++)
			{
				context.Jobs.Add(new Job
				{
					Title = possibleJobs[i],
					OrganisationId = hierchicalOrganisationID,
					Weight = randomNumberGenerator.Next(100)
				});
			}

			context.SaveChanges();

			firstJobInHierarchicalID = lastJobInProjectBasedID + 1;
			lastJobInHierarchicalID = firstJobInProjectBasedID + context.Jobs.Count() - 1;
		}
		private static void SeedJobsForProjectBasedOrganisation(int numberOfJobs)
		{

			if (numberOfJobs > maxNumberOfJobs)
			{
				throw new ArgumentException("You can have at most 50 jobs.");
			}

			for (int i = 0; i < numberOfJobs; i++)
			{
				context.Jobs.Add(new Job
				{
					Title = possibleJobs[i],
					OrganisationId = projectbasedOrganisationID,
					Weight = randomNumberGenerator.Next(100)
				});
			}

			context.SaveChanges();

			firstJobInProjectBasedID = context.Jobs.First().Id;
			lastJobInProjectBasedID = firstJobInProjectBasedID + context.Jobs.Count() - 1;
		}
		private static void SeedProjectsForProjectBasedOrganisation(int numberOfProjects)
		{
			for (int i = 0; i < numberOfProjects; i++)
			{
				context.Projects.Add(new Project
				{
					Name = possibleProjects[i],
					OrganisationId = projectbasedOrganisationID,
					Weight = randomNumberGenerator.Next(100)
				});
			}

			context.SaveChanges();

			firstProjectID = context.Projects.First().Id;
			lastProjectID = firstProjectID + context.Projects.Count() - 1;
		}
		private static void SeedMemberOfProjects()
		{
			int numberOfMembers = context.Members.Count();
			int numberOfProjects = context.Projects.Count();
			int maxNumberOfMemberOfProjects = numberOfProjects * numberOfMembers / 2;

			var memberOfProjects = new List<MemberOfProject>(maxNumberOfMemberOfProjects);

			for (int i = 0; i < maxNumberOfMemberOfProjects; i++)
			{
				int memberId = randomNumberGenerator.Next(firstMemberInProjectBasedID, lastMemberInProjectBasedID); // numberOfMembers/2: mert előbb szúrtuk be a hierarchikus organisation member-jeit
				int projectId = randomNumberGenerator.Next(firstProjectID, lastProjectID);

				if (!memberOfProjects.Any(m => m.MemberId == memberId && m.ProjectId == projectId))
				{
					memberOfProjects.Add(new MemberOfProject
					{
						MemberId = memberId,
						ProjectId = projectId
					});
				}
			}

			foreach (var memberOfProj in memberOfProjects)
            {
				context.MemberOfProjects.Add(memberOfProj);
            }
			
			context.SaveChanges();
		}
		private static void SeedEvents()
		{
			context.Events.Add(new Event {
				OrganisationId = hierchicalOrganisationID,
				Name = "Első esemény",
				DeadlineForApplication = new DateTime(2021, 03, 28),
				StartDate = new DateTime(2021, 03, 30),
				EndDate = new DateTime(2021, 04, 2),
			});
			context.Events.Add(new Event { 
				OrganisationId = hierchicalOrganisationID,
				Name = "Második esemény",
				DeadlineForApplication = new DateTime(2021, 04, 28),
				StartDate = new DateTime(2021, 04, 30),
				EndDate = new DateTime(2021, 05, 2)
			});
			context.Events.Add(new Event { 
				OrganisationId = hierchicalOrganisationID,
				Name = "Harmadik esemény",
				DeadlineForApplication = new DateTime(2021, 03, 28),
				StartDate = new DateTime(2021, 03, 30),
				EndDate = new DateTime(2021, 04, 2)
			});
			context.Events.Add(new Event { 
				OrganisationId = projectbasedOrganisationID,
				Name = "Negyedik esemény",
				DeadlineForApplication = new DateTime(2021, 04, 28),
				StartDate = new DateTime(2021, 04, 30),
				EndDate = new DateTime(2021, 05, 2)
			});
			context.Events.Add(new Event { 
				OrganisationId = projectbasedOrganisationID,
				Name = "Ötödik esemény",
				DeadlineForApplication = new DateTime(2021, 03, 28),
				StartDate = new DateTime(2021, 03, 30),
				EndDate = new DateTime(2021, 04, 2)
			});

			context.SaveChanges();
		}
		private static void SeedUsers()
		{
			var adminUser = new User
			{
				UserName = "adminH",
				Name = "Adminisztrátor",
				Email = "admin@example.com",
				PhoneNumber = "+36123456789",
				Address = "Nevesincs utca 1."
			};
			var adminPassword = "Almafa123";
			var adminRole = new IdentityRole<int>("administrator");

			var result1 = userManager.CreateAsync(adminUser, adminPassword).Result;
			var result2 = roleManager.CreateAsync(adminRole).Result;
			var result3 = userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;

			var adminUser2 = new User
			{
				UserName = "adminP",
				Name = "Adminisztrátor",
				Email = "a@a.a",
				PhoneNumber = "+36123456789",
				Address = "Nevesincs utca 1."
			};
			var adminPassword2 = "Almafa123";

			var result4 = userManager.CreateAsync(adminUser2, adminPassword2).Result;
			var result5 = userManager.AddToRoleAsync(adminUser2, adminRole.Name).Result;

			var user = new User
			{
				UserName = "user00",
				Name = "Valami User",
				Email = "user@example.com",
				PhoneNumber = "+36123456789",
				Address = "User utca 1."
			};
			var userPassword = "Almafa123";
			var userRole = new IdentityRole<int>("user");

			var result6 = userManager.CreateAsync(user, userPassword).Result;
			var result7 = roleManager.CreateAsync(userRole).Result;
			var result8 = userManager.AddToRoleAsync(user, userRole.Name).Result;

		}
		private static void SeedVenues()
        {
			var eventIds = context.Events.Select(o => o.Id).ToList();

			context.Venues.Add(
				new Venue
				{
					EventId = eventIds[0],
					Description = "Első mondat. Második mondat.",
					Address = "1000 Budapest, Valami utca 10.",
					LocationX = 45.4591,
					LocationY = 12.5068,
					GuestLimit = 30,
					Name = "Első helyszín"
				});
			context.Venues.Add(
				new Venue
				{
					EventId = eventIds[0],
					Description = "Első mondat. Második mondat.",
					Address = "1000 Budapest, Második utca 10.",
					LocationX = 45.4591,
					LocationY = 10.5068,
					GuestLimit = 30,
					Name = "Második helyszín"
				});
			context.Venues.Add(
				new Venue
				{
					EventId = eventIds[2],
					Description = "Első mondat. Második mondat.",
					Address = "1000 Budapest, Harmadik utca 10.",
					LocationX = 46.4591,
					LocationY = 11.5068,
					GuestLimit = 30,
					Name = "Harmadik helyszín"
				});

			context.SaveChanges();
		}
		private static void SeedVenueImages(string imageDirectory)
		{
			var venueIds = context.Venues.Select(o => o.Id).ToList();

			if (Directory.Exists(imageDirectory))
			{
				var images = new List<VenueImage>();

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
		/*
		private static void SeedMemberships()
        {
			var memberships = new Membership[]
			{
				new Membership
				{
					MemberId = context.Members.ElementAt(0).Id,
					UserId = context.Users.ElementAt(1).Id
				}
			};
        }*/
	}
}
