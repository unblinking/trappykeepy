using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using TrappyKeepy.Api.Controllers;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;
using Xunit;

namespace TrappyKeepy.Test.Controller
{
    public class SessionControllerTests
    {
        #region CREATE

        [Fact]
        public async Task CreateSessionWithServiceSuccessShouldReturnOkWithToken()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mock.
            var token = "faketokenstring";
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.CreateSession(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(
                    new UserServiceResponse()
                    {
                        Outcome = OutcomeType.Success,
                        Token = token
                    }
                );
            // Instantiate the production controller using the mock service.
            var sessionController = new SessionController(userServiceMock.Object);

            // ---------- ACT ----------
            var actionResult = await sessionController.Create(
                new UserSessionDto("foo@example.com", "passwordfoo")
            );

            // ---------- ASSERT ----------
            // ActionResult: The controller returns an ActionResult.
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult);
            // OkObjectResult: When the service outcome is success, the ActionResult should be OkObjectResult.
            var okObjectResult = (OkObjectResult)actionResult;
            Assert.NotNull(okObjectResult);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult.Value);
            Assert.IsType<ControllerResponse>(okObjectResult.Value);
            // ControllerResponse
            var controllerResponse = (ControllerResponse)okObjectResult.Value;
            Assert.NotNull(controllerResponse);
            Assert.Equal("success", controllerResponse.Status);
            Assert.Equal(token, controllerResponse.Data);
        }

        [Fact]
        public async Task CreateSessionWithServiceFailShouldReturnBadRequestWithErrorMessage()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mock.
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.CreateSession(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(
                    new UserServiceResponse()
                    {
                        Outcome = OutcomeType.Fail,
                        ErrorMessage = "No match found for Email and Password."
                    }
                );
            // Instantiate the production controller using the mock service.
            var sessionController = new SessionController(userServiceMock.Object);

            // ---------- ACT ----------
            var actionResult = await sessionController.Create(
                new UserSessionDto("foo@example.com", "passwordfoo")
            );

            // ---------- ASSERT ----------
            // ActionResult: The controller returns an ActionResult.
            Assert.NotNull(actionResult);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            // BadRequestObjectResult: When the service outcome is fail, the ActionResult should be BadRequestObjectResult.
            var badRequestObjectResult = (BadRequestObjectResult)actionResult;
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.NotNull(badRequestObjectResult.Value);
            Assert.IsType<ControllerResponse>(badRequestObjectResult.Value);
            // ControllerResponse
            var controllerResponse = (ControllerResponse)badRequestObjectResult.Value;
            Assert.NotNull(controllerResponse);
            Assert.Equal("fail", controllerResponse.Status);
            Assert.Equal("No match found for Email and Password.", controllerResponse.Message);
        }

        [Fact]
        public async Task CreateSessionWithServiceErrorShouldReturnObjectResult()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mock.
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.CreateSession(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(
                    new UserServiceResponse()
                    {
                        Outcome = OutcomeType.Error
                    }
                );
            // Instantiate the production controller using the mock service.
            var sessionController = new SessionController(userServiceMock.Object);

            // ---------- ACT ----------
            var actionResult = await sessionController.Create(
                new UserSessionDto("foo@example.com", "passwordfoo")
            );

            // ---------- ASSERT ----------
            // ActionResult: The controller returns an ActionResult.
            Assert.NotNull(actionResult);
            Assert.IsType<ObjectResult>(actionResult);
            // ObjectResult: When the service outcome is error, the ActionResult should be ObjectResult.
            var objectResult = (ObjectResult)actionResult;
            Assert.NotNull(objectResult);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.IsType<ControllerResponse>(objectResult.Value);
            // ControllerResponse
            var controllerResponse = (ControllerResponse)objectResult.Value;
            Assert.NotNull(controllerResponse);
            Assert.Equal("error", controllerResponse.Status);
            Assert.Equal("An error occurred while processing your request.", controllerResponse.Message);
        }

        #endregion CREATE
    }
}
