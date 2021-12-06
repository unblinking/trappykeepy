using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrappyKeepy.Api.Controllers;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Test.TestObjects;
using Xunit;

namespace TrappyKeepy.Test
{
    public class UserControllerTest
    {

        // TODO: Once I have the user roles, the user controller methods will
        // TODO: need to respond differently based on the requesting user role.
        // TODO: I'll stop here for now, so that I can get those parts done
        // TODO: first, but here's a unit test that mocks the service for the
        // TODO: controller.
        /*
                [Fact]
                public async Task ReadAllWithAnyRequestShouldReturnSuccessAllUsers()
                {
                    // ARRANGE
                    // Prepare the user service response.
                    var userServiceResponse = new UserServiceResponse();
                    var UserDtoTestObjects = new UserDtoTestObjects();
                    userServiceResponse.List = UserDtoTestObjects.TestUserDtoReadAll;
                    userServiceResponse.Outcome = OutcomeType.Success;
                    // Prepare the user service mock.
                    var userServiceMock = new Mock<IUserService>();
                    userServiceMock
                        .Setup(u => u.ReadAll(It.IsAny<UserServiceRequest>()))
                        .ReturnsAsync(userServiceResponse);
                    // Instantiate the production user controller using the mock user service.
                    var userController = new UserController(userServiceMock.Object);

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
        */
    }
}
