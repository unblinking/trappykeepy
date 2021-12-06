using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrappyKeepy.Api.Controllers;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Test.TestObjects;
using Xunit;

namespace TrappyKeepy.Test
{
    public class UserControllerTest
    {


        [Fact]
        public async Task ReadAllWithAnyRequestShouldReturnSuccessAllUsers()
        {
            // ARRANGE
            // Prepare the service response.
            var userServiceResponse = new UserServiceResponse();
            var UserDtoTestObjects = new UserDtoTestObjects();
            userServiceResponse.List = UserDtoTestObjects.TestUserDtoReadAll;
            userServiceResponse.Outcome = OutcomeType.Success;
            // Prepare the service mocks.
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.ReadAll(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(userServiceResponse);

            // Instantiate the production controller using the mock services.
            var userController = new UserController(membershipServiceMock.Object, permitServiceMock.Object, userServiceMock.Object);

            // ACT
            var actionResult = await userController.ReadAll();

            // ASSERT
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(200, ((OkObjectResult)actionResult).StatusCode);
            Assert.IsType<ControllerResponse>(((OkObjectResult)actionResult).Value);
            Assert.NotNull(((OkObjectResult)actionResult).Value);
            var controllerResponse = (ControllerResponse)((OkObjectResult)actionResult).Value;
            var controllerResponseStatus = controllerResponse.Status;
            Assert.Equal("success", controllerResponseStatus);
            var controllerResponseData = (List<UserDto>)controllerResponse.Data;
            Assert.Equal(userServiceResponse.List, controllerResponseData);
        }
    }
}
