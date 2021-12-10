using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using TrappyKeepy.Api.Controllers;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Test.TestObjects;
using Xunit;

namespace TrappyKeepy.Test.Controller
{
    public class SessionControllerTests
    {
        private DtoTestObjects _dto;
        private Mock<IUserService> _userService;

        public SessionControllerTests()
        {
            _dto = new DtoTestObjects();
            _userService = new Mock<IUserService>();
        }

        private void RefreshMocks()
        {
            _userService = new Mock<IUserService>();
        }

        #region CREATE

        [Fact]
        public async Task CreateSessionWithServiceSuccessShouldReturnOkWithToken()
        {
            // ---------- ARRANGE ----------
            RefreshMocks();
            var userSessionDto = _dto.TestUserSessionDto;
            var token = "faketokenstring";
            var response = new UserServiceResponse() { Outcome = OutcomeType.Success, Token = token };
            _userService.Setup(u => u.CreateSession(It.IsAny<UserServiceRequest>())).ReturnsAsync(response);
            var sessionController = new SessionController(_userService.Object);

            // ---------- ACT ----------
            var actionResult = await sessionController.Create((UserSessionDto)userSessionDto);

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
            RefreshMocks();
            var userSessionDto = _dto.TestUserSessionDto;
            var response = new UserServiceResponse() { Outcome = OutcomeType.Fail, ErrorMessage = "No match found for Email and Password." };
            _userService.Setup(u => u.CreateSession(It.IsAny<UserServiceRequest>())).ReturnsAsync(response);
            var sessionController = new SessionController(_userService.Object);

            // ---------- ACT ----------
            var actionResult = await sessionController.Create((UserSessionDto)userSessionDto);

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
            RefreshMocks();
            var userSessionDto = _dto.TestUserSessionDto;
            var response = new UserServiceResponse() { Outcome = OutcomeType.Error };
            _userService.Setup(u => u.CreateSession(It.IsAny<UserServiceRequest>())).ReturnsAsync(response);
            var sessionController = new SessionController(_userService.Object);

            // ---------- ACT ----------
            var actionResult = await sessionController.Create((UserSessionDto)userSessionDto);

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
