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
		private static MeetingApplicationContext context;
		private static UserManager<User> userManager;
		private static RoleManager<IdentityRole<int>> roleManager;

		public static void Initialize(IServiceProvider serviceProvider, string imageDirectory)
		{
			context = serviceProvider.GetRequiredService<MeetingApplicationContext>();
			userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

			context.Database.Migrate();

			if (!context.Users.Any())
            {
				SeedUsers();
            }
			if (!context.Organisations.Any())
			{
				SeedOrganisations();
				SeedEvents();
				SeedVenues();
			}
            if (!context.Members.Any())
            {
				SeedMembers();
            }
            if (!context.AcceptedEmailDomains.Any())
            {
				SeedAcceptedEmailDomains();
            }
            if (!context.Jobs.Any())
            {
				SeedJobs();
            }
            if (!context.Projects.Any())
            {
				SeedProjects();
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
			foreach (var j in jobs)
			{
				context.Jobs.Add(j);
			}

			context.SaveChanges();
		}
		private static void SeedProjects()
		{
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();

			Project[] projects = new Project[]
			{
				new Project
				{
					Name = "Első projekt",
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

			foreach(var p in projects)
            {
				context.Projects.Add(p);
            }
		}
		private static void SeedAcceptedEmailDomains()
        {
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();

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
				context.AcceptedEmailDomains.Add(d);
			}

			context.SaveChanges();
		}
		private static void SeedOrganisations()
		{
			var orgs = new Organisation[]
			{
				new Organisation {
					Name = "Lorem",
					TypeOfStructure = TypeOfStructure.Hierarchical,
					Description = "Lorem az valami cég.",
					PermitNewMembers = true,
					Address = "Egyik utca 2.",
					AdminId = context.Users.First(u => u.UserName == "adminLorem").Id
				},
				new Organisation {
					Name = "Ipsum",
					TypeOfStructure = TypeOfStructure.Hierarchical,
					Description = "Az Ipsum az valami másik cég.",
					PermitNewMembers = false,
					Address = "Másik utca 7.",
					AdminId = context.Users.First(u => u.UserName == "adminIpsum").Id
				}
			};
			foreach (var o in orgs)
			{
				context.Organisations.Add(o);
			}

			context.SaveChanges();
		}
		private static void SeedEvents()
		{
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();

			context.Events.Add(new Event {
				OrganisationId = organisationIds[0],
				Name = "Első esemény",
				DeadlineForApplication = new DateTime(2021, 03, 28),
				StartDate = new DateTime(2021, 03, 30),
				EndDate = new DateTime(2021, 04, 2),
			});
			context.Events.Add(new Event { 
				OrganisationId = organisationIds[0],
				Name = "Második esemény",
				DeadlineForApplication = new DateTime(2021, 04, 28),
				StartDate = new DateTime(2021, 04, 30),
				EndDate = new DateTime(2021, 05, 2)
			});
			context.Events.Add(new Event { 
				OrganisationId = organisationIds[0],
				Name = "Harmadik esemény",
				DeadlineForApplication = new DateTime(2021, 03, 28),
				StartDate = new DateTime(2021, 03, 30),
				EndDate = new DateTime(2021, 04, 2)
			});
			context.Events.Add(new Event { 
				OrganisationId = organisationIds[1],
				Name = "Negyedik esemény",
				DeadlineForApplication = new DateTime(2021, 04, 28),
				StartDate = new DateTime(2021, 04, 30),
				EndDate = new DateTime(2021, 05, 2)
			});
			context.Events.Add(new Event { 
				OrganisationId = organisationIds[1],
				Name = "Ötödik esemény",
				DeadlineForApplication = new DateTime(2021, 03, 28),
				StartDate = new DateTime(2021, 03, 30),
				EndDate = new DateTime(2021, 04, 2)
			});
			context.SaveChanges();

			context.SaveChanges();
		}
		private static void SeedMembers()
		{
			var organisationIds = context.Organisations.Select(o => o.Id).ToList();
			var members = new Member[]
			{
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "akarki@gmail.com",
					Name = "Nem Tomi"
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "akark@gmail.com",
					Name = "Nem Tom"
				},
				new Member
				{
					OrganisationId = organisationIds[0],
					Email = "akar@gmail.com",
					Name = "Nem To"
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "akark@gmail.com",
					Name = "Nem Tom"
				},
				new Member
				{
					OrganisationId = organisationIds[1],
					Email = "akarki@gmail.com",
					Name = "Nem Tomi"
				},
			};
			foreach (var member in members)
            {
				context.Add(member);
            }
			context.SaveChanges();

		}/*
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
		private static void SeedUsers()
		{
			var adminUser = new User
			{
				UserName = "adminLorem",
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
				UserName = "adminIpsum",
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
