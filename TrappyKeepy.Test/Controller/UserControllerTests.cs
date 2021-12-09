using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
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

        [Fact]
        public async Task CreateUserWithServiceSuccessShouldReturnOkWithUser()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mocks.
            var dtoTestObjects = new DtoTestObjects();
            var user = dtoTestObjects.TestUserDto;
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.Create(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(
                    new UserServiceResponse()
                    {
                        Outcome = OutcomeType.Success,
                        Item = user
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.CreateUser(
                new UserDto()
                {
                    Name = "foo",
                    Password = "passwordfoo",
                    Email = "foo@trappykeepy.com",
                    Role = "basic"
                }
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
            Assert.NotNull(controllerResponse.Data);
            Assert.IsType<UserDto>(controllerResponse.Data);
            var data = (IUserDto)controllerResponse.Data;
            Assert.Equal(user, data);
        }

        [Fact]
        public async Task CreateUserWithServiceFailShouldReturnBadRequestWithErrorMessage()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mocks.
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.Create(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(
                    new UserServiceResponse()
                    {
                        Outcome = OutcomeType.Fail,
                        ErrorMessage = "Requested user name is already in use."
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.CreateUser(
                new UserDto()
                {
                    Name = "foo",
                    Password = "passwordfoo",
                    Email = "foo@trappykeepy.com",
                    Role = "basic"
                }
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
            Assert.Equal("Requested user name is already in use.", controllerResponse.Message);
        }

        [Fact]
        public async Task CreateUserWithServiceErrorShouldReturnObjectResult()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mocks.
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(u => u.Create(It.IsAny<UserServiceRequest>()))
                .ReturnsAsync(
                    new UserServiceResponse()
                    {
                        Outcome = OutcomeType.Error
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.CreateUser(
                new UserDto()
                {
                    Name = "foo",
                    Password = "passwordfoo",
                    Email = "foo@trappykeepy.com",
                    Role = "basic"
                }
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

        public async Task CreateUserMembershipWithServiceSuccessShouldReturnOkWithMembership()
        {
            // ---------- ARRANGE ----------
            // Prepare some local variables.
            var dtoTestObjects = new DtoTestObjects();
            var membership = dtoTestObjects.TestMembershipDto;
            var userId = Guid.NewGuid();
            // Prepare the service mocks.
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            membershipServiceMock
                .Setup(u => u.Create(It.IsAny<MembershipServiceRequest>()))
                .ReturnsAsync(
                    new MembershipServiceResponse()
                    {
                        Outcome = OutcomeType.Success,
                        Item = membership
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserMembership(
                userId,
                new MembershipDto()
                {
                    GroupId = Guid.NewGuid(),
                    UserId = userId
                }
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
            Assert.NotNull(controllerResponse.Data);
            Assert.IsType<MembershipDto>(controllerResponse.Data);
            var data = (IMembershipDto)controllerResponse.Data;
            Assert.Equal(membership, data);
        }

        public async Task CreateUserMembershipWithServiceFailShouldReturnBadRequestWithErrorMessage()
        {
            // ---------- ARRANGE ----------
            // Prepare some local variables.
            var dtoTestObjects = new DtoTestObjects();
            var membership = dtoTestObjects.TestMembershipDto;
            var userId = Guid.NewGuid();
            // Prepare the service mocks.
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            membershipServiceMock
                .Setup(u => u.Create(It.IsAny<MembershipServiceRequest>()))
                .ReturnsAsync(
                    new MembershipServiceResponse()
                    {
                        Outcome = OutcomeType.Fail,
                        ErrorMessage = "Requested membership already exists."
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserMembership(
                userId,
                new MembershipDto()
                {
                    GroupId = Guid.NewGuid(),
                    UserId = userId
                }
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
            Assert.Equal("Requested membership already exists.", controllerResponse.Message);
        }

        public async Task CreateUserMembershipWithServiceErrorShouldReturnObjectResult()
        {
            // ---------- ARRANGE ----------
            // Prepare some local variables.
            var dtoTestObjects = new DtoTestObjects();
            var membership = dtoTestObjects.TestMembershipDto;
            var userId = Guid.NewGuid();
            // Prepare the service mocks.
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            membershipServiceMock
                .Setup(u => u.Create(It.IsAny<MembershipServiceRequest>()))
                .ReturnsAsync(
                    new MembershipServiceResponse()
                    {
                        Outcome = OutcomeType.Error
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserMembership(
                userId,
                new MembershipDto()
                {
                    GroupId = Guid.NewGuid(),
                    UserId = userId
                }
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

        public async Task CreateUserPermitWithServiceSuccessShouldReturnOkWithPermit()
        {
            // ---------- ARRANGE ----------
            // Prepare some local variables.
            var dtoTestObjects = new DtoTestObjects();
            var permit = dtoTestObjects.TestPermitDto;
            var userId = Guid.NewGuid();
            // Prepare the service mocks.
            var membershipServiceMock = new Mock<IMembershipService>();
            var permitServiceMock = new Mock<IPermitService>();
            var userServiceMock = new Mock<IUserService>();
            permitServiceMock
                .Setup(u => u.Create(It.IsAny<PermitServiceRequest>()))
                .ReturnsAsync(
                    new PermitServiceResponse()
                    {
                        Outcome = OutcomeType.Success,
                        Item = permit
                    }
                );
            // Instantiate the production controller using the mock services.
            var userController = new UserController(
                membershipServiceMock.Object,
                permitServiceMock.Object,
                userServiceMock.Object
            );

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserPermit(
                userId,
                new PermitDto()
                {
                    KeeperId = Guid.NewGuid(),
                    UserId = userId,
                    GroupId = Guid.NewGuid()
                }
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
            Assert.NotNull(controllerResponse.Data);
            Assert.IsType<PermitDto>(controllerResponse.Data);
            var data = (IPermitDto)controllerResponse.Data;
            Assert.Equal(permit, data);
        }


        #endregion CREATE

        #region READ

        [Fact]
        public async Task ReadAllWithServiceSuccessShouldReturnOkWithUsersList()
        {
            // ---------- ARRANGE ----------
            // Prepare the service mocks.
            var dtoTestObjects = new DtoTestObjects();
            var userList = dtoTestObjects.TestUserDtoList;
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
            Assert.IsType<List<IUserDto>>(controllerResponse.Data);
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
