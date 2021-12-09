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
        private DtoTestObjects _dto;
        private Mock<IMembershipService> _membershipService;
        private Mock<IPermitService> _permitService;
        private Mock<IUserService> _userService;

        public UserControllerTest()
        {
            _dto = new DtoTestObjects();
            _membershipService = new Mock<IMembershipService>();
            _permitService = new Mock<IPermitService>();
            _userService = new Mock<IUserService>();
        }

        public void RefreshMocks()
        {
            _membershipService = new Mock<IMembershipService>();
            _permitService = new Mock<IPermitService>();
            _userService = new Mock<IUserService>();
        }

        # region CREATE

        [Fact]
        public async Task CreateUserWithServiceSuccessShouldReturnOkWithUser()
        {
            // ---------- ARRANGE ----------
            RefreshMocks();
            var user = _dto.TestUserDto;
            var response = new UserServiceResponse() { Outcome = OutcomeType.Success, Item = user };
            _userService.Setup(u => u.Create(It.IsAny<UserServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

            // ---------- ACT ----------
            var actionResult = await userController.CreateUser((UserDto)user);

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
            RefreshMocks();
            var user = _dto.TestUserDto;
            var response = new UserServiceResponse() { Outcome = OutcomeType.Fail, ErrorMessage = "Requested user name is already in use." };
            _userService.Setup(u => u.Create(It.IsAny<UserServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

            // ---------- ACT ----------
            var actionResult = await userController.CreateUser((UserDto)user);

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
            RefreshMocks();
            var user = _dto.TestUserDto;
            var response = new UserServiceResponse() { Outcome = OutcomeType.Error };
            _userService.Setup(u => u.Create(It.IsAny<UserServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

            // ---------- ACT ----------
            var actionResult = await userController.CreateUser((UserDto)user);

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
            RefreshMocks();
            var membership = _dto.TestMembershipDto;
            var userId = Guid.NewGuid();
            var response = new MembershipServiceResponse() { Outcome = OutcomeType.Success, Item = membership };
            _membershipService.Setup(u => u.Create(It.IsAny<MembershipServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserMembership(userId, (MembershipDto)membership);

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
            RefreshMocks();
            var membership = _dto.TestMembershipDto;
            var userId = Guid.NewGuid();
            var response = new MembershipServiceResponse() { Outcome = OutcomeType.Fail, ErrorMessage = "Requested membership already exists." };
            _membershipService.Setup(u => u.Create(It.IsAny<MembershipServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserMembership(userId, (MembershipDto)membership);

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
            RefreshMocks();
            var membership = _dto.TestMembershipDto;
            var userId = Guid.NewGuid();
            var response = new MembershipServiceResponse() { Outcome = OutcomeType.Error };
            _membershipService.Setup(u => u.Create(It.IsAny<MembershipServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserMembership(userId, (MembershipDto)membership);

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
            RefreshMocks();
            var permit = _dto.TestPermitDto;
            var userId = Guid.NewGuid();
            var response = new PermitServiceResponse() { Outcome = OutcomeType.Success, Item = permit };
            _permitService.Setup(u => u.Create(It.IsAny<PermitServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

            // ---------- ACT ----------
            var actionResult = await userController.CreateUserPermit(userId, (PermitDto)permit);

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

        // TODO: Finish the fail and error CreateUserPermit tests.

        #endregion CREATE

        #region READ

        [Fact]
        public async Task ReadAllWithServiceSuccessShouldReturnOkWithUsersList()
        {
            // ---------- ARRANGE ----------
            RefreshMocks();
            var userList = _dto.TestUserDtoList;
            var response = new UserServiceResponse() { Outcome = OutcomeType.Success, List = userList };
            _userService.Setup(u => u.ReadAll(It.IsAny<UserServiceRequest>())).ReturnsAsync(response);
            var userController = new UserController(_membershipService.Object, _permitService.Object, _userService.Object);

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

        // TODO: Finish the fail and error ReadAll tests.

        #endregion READ

        #region UPDATE



        #endregion UPDATE

        #region DELETE



        #endregion DELETE
    }
}
