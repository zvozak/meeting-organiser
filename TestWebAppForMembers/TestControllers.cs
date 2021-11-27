using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using CommonData;
using WebAppForMembers.Controllers;
using WebAppForMembers.Models;
using CommonData.Entities;
using WebAppForMembers;

namespace TestWebAppForMembers
{
    [TestClass]
    public class TestControllers
    {
        Mock<IServiceForMembers> serviceMock;
        Mock<UserManager<User>> userManagerMock;
        Mock<OptionsManager<GoogleConfig>> googleConfigMock;
        EventsController eventsController;
        JoinOrganisationController joinOrganisationController;

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
            userManagerMock = MockUserManager();
            var contextMock = new Mock<MeetingApplicationContext>();
            serviceMock = new Mock<IServiceForMembers>();
            googleConfigMock = new Mock<OptionsManager<GoogleConfig>>();
            eventsController = new EventsController(serviceMock.Object, userManagerMock.Object, null);
            joinOrganisationController = new JoinOrganisationController(serviceMock.Object, userManagerMock.Object);
        }

        [TestMethod] // DATA DRIVEN TEST
        public void TestEventsController_VerifyStartDate()
        {
            int numberOfTestCases = 9;
            var expectedResults = new List<bool>(numberOfTestCases);
            Dictionary<String, DateTime>[] inputs = new Dictionary<String, DateTime>[numberOfTestCases];
            var fixedStartDateOfEvent = new DateTime(2022, 1, 1, 10, 0, 0);
            var fixedEndDateOfEvent = new DateTime(2022, 2, 11, 8, 0, 0);
            var anHourBeforeFixedStartDateOfEvent = new DateTime(2022, 1, 1, 9, 0, 0);
            var anHourAfterFixedStartDateOfEvent = new DateTime(2022, 1, 1, 11, 0, 0);
            var anHourBeforeFixedEndDateOfEvent = new DateTime(2022, 2, 11, 7, 0, 0);
            var anHourAfterFixedEndDateOfEvent = new DateTime(2022, 2, 11, 9, 0, 0);
            var middleOfEvent = new DateTime(2022, 1, 20, 12, 0, 0);
            var anHourAfterMiddleOfEvent = new DateTime(2022, 1, 20, 13, 0, 0);
            String keyOfStartDateOfEvent = "startDateOfEvent";
            String keyOfEndDateOfEvent = "endDateOfEvent";
            String keyOfStartDate = "startDate";
            String keyOfEndDate = "endDate";
            for (int i = 0; i < numberOfTestCases; i++)
            {
                inputs[i] = new Dictionary<string, DateTime>();
            }
            inputs[0].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(false);
            inputs[0].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[0].Add(keyOfStartDate, anHourBeforeFixedStartDateOfEvent);
            inputs[0].Add(keyOfEndDate, fixedEndDateOfEvent);
            inputs[1].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(true); //mert csak a kezdő dátumot ellenőrizzük
            inputs[1].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[1].Add(keyOfStartDate, fixedStartDateOfEvent);
            inputs[1].Add(keyOfEndDate, anHourAfterFixedEndDateOfEvent);
            inputs[2].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(true); //mert csak a kezdő dátumot ellenőrizzük
            inputs[2].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[2].Add(keyOfStartDate, anHourAfterFixedStartDateOfEvent);
            inputs[2].Add(keyOfEndDate, anHourAfterFixedEndDateOfEvent);
            inputs[3].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(false);
            inputs[3].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[3].Add(keyOfStartDate, anHourAfterMiddleOfEvent);
            inputs[3].Add(keyOfEndDate, middleOfEvent);
            inputs[4].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(true);
            inputs[4].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[4].Add(keyOfStartDate, middleOfEvent);
            inputs[4].Add(keyOfEndDate, middleOfEvent);
            inputs[5].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(true);
            inputs[5].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[5].Add(keyOfStartDate, anHourBeforeFixedEndDateOfEvent);
            inputs[5].Add(keyOfEndDate, anHourAfterFixedEndDateOfEvent);
            inputs[6].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(true);
            inputs[6].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[6].Add(keyOfStartDate, fixedEndDateOfEvent);
            inputs[6].Add(keyOfEndDate, anHourAfterFixedEndDateOfEvent);
            inputs[7].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(false);
            inputs[7].Add(keyOfEndDateOfEvent, fixedEndDateOfEvent);
            inputs[7].Add(keyOfStartDate, anHourAfterFixedEndDateOfEvent);
            inputs[7].Add(keyOfEndDate, anHourAfterFixedEndDateOfEvent);
            inputs[8].Add(keyOfStartDateOfEvent, fixedStartDateOfEvent); expectedResults.Add(true);
            inputs[8].Add(keyOfEndDateOfEvent, fixedStartDateOfEvent);
            inputs[8].Add(keyOfStartDate, fixedStartDateOfEvent);
            inputs[8].Add(keyOfEndDate, anHourAfterFixedEndDateOfEvent);

            List<bool> actualResults = new List<bool>();
            for (int i = 0; i < numberOfTestCases; i++)
            {
                actualResults.Add((bool)eventsController.VerifyStartDate(
                    inputs[i][keyOfStartDate],
                    inputs[i][keyOfEndDate],
                    inputs[i][keyOfEndDateOfEvent],
                    inputs[i][keyOfStartDateOfEvent]).Value);
            }
            var l = new List<bool>(expectedResults);
            Assert.IsTrue(actualResults.SequenceEqual(expectedResults));
        }

        
        [TestMethod]
        public void TestJoinOrganisationController_VerifyProjectWhenProjectIsAlreadyRegisteredToTheGivenOrganisationShouldReturnFalse()
        {
            string inputProject = "An Old Project";
            var possibleProjectsAtOrganisation = new List<Project> {
                new Project{ Name = "Example Proj", OrganisationId = 1},
                new Project{ Name = inputProject,   OrganisationId = 1}
            };
            var otherProjectNames = new HashSet<string>();
            bool isNewProject = (bool)joinOrganisationController.VerifyProject(inputProject, possibleProjectsAtOrganisation, otherProjectNames).Value; ;

            Assert.IsFalse(isNewProject);
        }

        [TestMethod]
        public void TestJoinOrganisationController_VerifyProjectWhenProjectIsAlreadyInTheListOfYetToBeNewlyRegisteredProjectsShouldReturnFalse()
        {
            string inputProject = "An Old Project";
            var otherProjectNames = new HashSet<String> { "Example Proj", inputProject };
            var possibleProjectsAtOrganisation = new List<Project>();
            bool isNewProject = (bool)joinOrganisationController.VerifyProject(inputProject, possibleProjectsAtOrganisation, otherProjectNames).Value; ;

            Assert.IsFalse(isNewProject);
        }

        [TestMethod]
        public void TestJoinOrganisationController_VerifyProjectWhenProjectIsNotInEitherListShouldReturnTrue()
        {
            string inputProject = "A NEW Project";
            var possibleProjectsAtOrganisation = new List<Project> {
                new Project{ Name = "Example Proj", OrganisationId = 1},
                new Project{ Name = "An Old Project",   OrganisationId = 1}
            };
            var otherProjectNames = new HashSet<string> { "Example1", "example2" };
            bool isNewProject = (bool)joinOrganisationController.VerifyProject(inputProject, possibleProjectsAtOrganisation, otherProjectNames).Value; ;

            Assert.IsTrue(isNewProject);
        }

    }
}
