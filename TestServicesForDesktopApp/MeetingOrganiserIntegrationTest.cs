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
                },
                new Organisation
                {
                    Name = "Has No Events",
                    TypeOfStructure = TypeOfStructure.Hierarchical,
                    Description = "",
                    PermitNewMembers = false,
                    Address = "Másik utca 7.",
                    AdminId = 2,
                    Id = 3
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
                    EndDate = new DateTime(2021, 04, 2),
                    Id = 1
                },
                new Event
                {
                    OrganisationId = OrganisationData[0].Id,
                    Name = "Második esemény",
                    DeadlineForApplication = new DateTime(2021, 04, 28),
                    StartDate = new DateTime(2021, 04, 30),
                    EndDate = new DateTime(2021, 05, 2),
                    Id = 2
                },
                new Event
                {
                    OrganisationId = OrganisationData[0].Id,
                    Name = "Harmadik esemény",
                    DeadlineForApplication = new DateTime(2021, 03, 28),
                    StartDate = new DateTime(2021, 03, 30),
                    EndDate = new DateTime(2021, 04, 2),
                    Id = 3
                },
                new Event
                {
                    OrganisationId = OrganisationData[1].Id,
                    Name = "Negyedik esemény",
                    DeadlineForApplication = new DateTime(2021, 02, 28),
                    StartDate = new DateTime(2021, 04, 30),
                    EndDate = new DateTime(2021, 05, 2),
                    Id = 4
                },
                new Event
                {
                    OrganisationId = OrganisationData[1].Id,
                    Name = "Ötödik esemény",
                    DeadlineForApplication = new DateTime(2021, 06, 28),
                    StartDate = new DateTime(2021, 03, 30),
                    EndDate = new DateTime(2021, 04, 2),
                    Id = 5
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
        private readonly String notFoundResponse = "NotFound";


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
        public void TestGetOrganisationWhenOrganisationExistsShouldReturnOrganisation()
        {
            string existingOrganisationName = OrganisationData.ElementAt(0).Name;

            OrganisationDTO result = persistence.ReadOrganisationAsync(existingOrganisationName);

            Assert.Equal(organisationDTOs.ElementAt(0), result);
        }
        
        [Fact]
        public void TestGetOrganisationWhenOrganisationDoesNotExistShouldReturnPersistenceUnavailableExceptionWithNoutFoundInnerException()
        {
            string unknownOrganisationName = "Ismeretlen";
            string expectedErrorMessage = "Service returned response: " + notFoundResponse;
            PersistenceUnavailableException actualException = null;
            OrganisationDTO result = null;
            try
            {
                result = persistence.ReadOrganisationAsync(unknownOrganisationName);
            }
            catch(PersistenceUnavailableException e)
            {
                actualException = e;
            }

            Assert.Null(result);
            Assert.Contains(expectedErrorMessage, actualException.Message);
        }

        [Fact]
        public void TestGetEventsWhenOrganisationDoesNotExistShouldReturnEmptyList()
        {
            int unknownOrganisationID = 100;

            IEnumerable<EventDTO> result = persistence.ReadEventsAsync(unknownOrganisationID).Result;

            Assert.Empty(result);
        }

        [Fact]
        public void TestGetEventsWhenOrganisationHasNoEventsShouldReturnEmptyList()
        {
            int organisationWithoutEventsID = 3;

            IEnumerable<EventDTO> result = persistence.ReadEventsAsync(organisationWithoutEventsID).Result;

            Assert.Empty(result);
        }

        [Fact]
        public void TestGetEventsWhenOrganisationHasEventsShouldReturnItsEvents()
        {
            int organisationID = 1;
            var expectedResult = eventDTOs.GetRange(0, 3);

            List<EventDTO> result = persistence.ReadEventsAsync(organisationID).Result.ToList();

            Assert.True(result.SequenceEqual(expectedResult));
        }
        /* IN memory adatbázissal nem megy
        [Fact]
        public void TestPutEventWhenEventExistsShouldEditEvent()
        {
            var updatedEvent = eventDTOs.ElementAt(0);
            updatedEvent.Name = "New Name";
            updatedEvent.EndDate = new DateTime(2023, 1, 1, 10, 0, 0);

            bool successful = persistence.UpdateEventAsync(updatedEvent).Result;
            var actualEvent = persistence.ReadEventsAsync(updatedEvent.OrganisationId).Result.Single(e => e.Id == updatedEvent.Id);

            Assert.True(successful);
            Assert.Equal(updatedEvent, actualEvent);
        }
        [Fact]
        public void TestPutEventWhenEventDoesNotExistShouldReturnPersistenceUnavailableExceptionWithNoutFoundInnerException()
        {
            string unknownOrganisationName = "Ismeretlen";
            string expectedErrorMessage = "Service returned response: " + notFoundResponse;
            PersistenceUnavailableException actualException = null;
            OrganisationDTO result = null;
            try
            {
                result = persistence.ReadOrganisationAsync(unknownOrganisationName);
            }
            catch (PersistenceUnavailableException e)
            {
                actualException = e;
            }

            Assert.Null(result);
            Assert.Contains(expectedErrorMessage, actualException.Message);
        }
        */

    }
}
