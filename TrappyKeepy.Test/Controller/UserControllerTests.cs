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
        # region CREATE



        #endregion CREATE

        #region READ

        [Fact]
        public async Task ReadAllWithServiceSuccessShouldReturnOkWithUsersList()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mocks.
            var UserDtoTestObjects = new UserDtoTestObjects();
            var userList = UserDtoTestObjects.TestUserDtoReadAll;
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.ReadAll(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(
                    new UserServiceResponse()
                    {
                        Outcome = OutcomeType.Success,
                        List = userList
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.ReadAll();

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
            Assert.NotNull(controllerResponse.Data);
            // Assert.IsType<List<IUserDto>>(controllerResponse.Data);
            var data = (List<IUserDto>)controllerResponse.Data;
            Assert.Equal(userList, data);
        }

        #endregion READ

        #region UPDATE



        #endregion UPDATE

        #region DELETE



        #endregion DELETE
    }
}
