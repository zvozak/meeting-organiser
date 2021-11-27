using CommonData;
using CommonData.DTOs;
using CommonData.Entities;
using MeetingOrganiserDesktopApp.Persistence;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace TestServicesForDesktopApp
{
    public class MeetingOrganiserIntegrationTest : IDisposable
    {

        public static IList<Organisation> OrganisationData = new List<Organisation>
            {
                new Organisation {
                    Name = "Lorem",
                    TypeOfStructure = TypeOfStructure.Hierarchical,
                    Description = "Lorem az valami cég.",
                    PermitNewMembers = true,
                    Address = "Egyik utca 2.",
                    AdminId = 1
                },
                new Organisation {
                    Name = "Ipsum",
                    TypeOfStructure = TypeOfStructure.Hierarchical,
                    Description = "Az Ipsum az valami másik cég.",
                    PermitNewMembers = false,
                    Address = "Másik utca 7.",
                    AdminId = 2
                }
        };

        public static IList<Event> EventData = new List<Event>
        {
                new Event
                {
                    OrganisationId = OrganisationData[0].Id,
                    Name = "Első esemény",
                    DeadlineForApplication = new DateTime(2021, 03, 28),
                    StartDate = new DateTime(2021, 03, 30),
                    EndDate = new DateTime(2021, 04, 2)
                },
                new Event
                {
                    OrganisationId = OrganisationData[0].Id,
                    Name = "Második esemény",
                    DeadlineForApplication = new DateTime(2021, 04, 28),
                    StartDate = new DateTime(2021, 04, 30),
                    EndDate = new DateTime(2021, 05, 2)
                },
                new Event
                {
                    OrganisationId = OrganisationData[0].Id,
                    Name = "Harmadik esemény",
                    DeadlineForApplication = new DateTime(2021, 03, 28),
                    StartDate = new DateTime(2021, 03, 30),
                    EndDate = new DateTime(2021, 04, 2)
                },
                new Event
                {
                    OrganisationId = OrganisationData[1].Id,
                    Name = "Negyedik esemény",
                    DeadlineForApplication = new DateTime(2021, 02, 28),
                    StartDate = new DateTime(2021, 04, 30),
                    EndDate = new DateTime(2021, 05, 2)
                },
                new Event
                {
                    OrganisationId = OrganisationData[1].Id,
                    Name = "Ötödik esemény",
                    DeadlineForApplication = new DateTime(2021, 06, 28),
                    StartDate = new DateTime(2021, 03, 30),
                    EndDate = new DateTime(2021, 04, 2)
                }
        };
        public static IList<User> UserData = new List<User>
        {
            new User
            {
                UserName = "admin",
                Name = "Adminisztrátor",
                Email = "admin@example.com",
                PhoneNumber = "+36123456789",
                Address = "Nevesincs utca 1."
            },
            new User
            {
                UserName = "user",
                Name = "Valami User",
                Email = "user@example.com",
                PhoneNumber = "+36123456789",
                Address = "User utca 1."
            }
        };
        public static String AdminPassword = "Almafa123";
        public static String UserPassword = "Almafa123";

        private readonly List<OrganisationDTO> organisationDTOs;
        private readonly List<EventDTO> eventDTOs;
        private readonly IMeetingApplicationPersistence persistence;

        private readonly IHost server;
        private readonly HttpClient client;
        public MeetingOrganiserIntegrationTest()
        {
            organisationDTOs = OrganisationData.Select(o => (OrganisationDTO)o).ToList();
            eventDTOs = EventData.Select(e => (EventDTO)e).ToList();

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost
                        .UseTestServer()
                        .UseStartup<TestStartup>()
                        .UseEnvironment("Development");
                });

            server = hostBuilder.Start();

            client = server.GetTestClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            persistence = new MeetingApplicationServicePersistence(client);
        }

        public void Dispose()
        {
            var dbContext = server.Services.GetRequiredService<MeetingApplicationContext>();
            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public void GetOrganisationTestWhenOrganisationExistsAndUserIsAdmin()
        {
            var adminUser = UserData.ElementAt(0);
            string existingOrganisationName = OrganisationData.ElementAt(0).Name;
            persistence.LoginAsync(adminUser.UserName, AdminPassword, existingOrganisationName);

            OrganisationDTO result = persistence.ReadOrganisationAsync(existingOrganisationName);

            // Assert
            Assert.Equal(organisationDTOs.ElementAt(0), result);
        }
        [Fact]
        public void GetOrganisationTestWhenOrganisationExistsButUserIsNotAdmin()
        {
            string existingOrganisationName = "Lorem";
            OrganisationDTO result = persistence.ReadOrganisationAsync(existingOrganisationName);

            // Assert
            Assert.Equal(organisationDTOs.ElementAt(0), result);
        }
        [Fact]
        public void GetOrganisationTestWhenOrganisationDoesNotExistAndUserIsAdmin()
        {
            string existingOrganisationName = "Lorem";
            OrganisationDTO result = persistence.ReadOrganisationAsync(existingOrganisationName);

            // Assert
            Assert.Equal(organisationDTOs.ElementAt(0), result);
        }

    }
}
