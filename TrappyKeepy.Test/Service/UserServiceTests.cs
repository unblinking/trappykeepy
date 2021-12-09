using AutoMapper;
using Moq;
using System.Threading.Tasks;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Service;
using TrappyKeepy.Test.TestObjects;
using Xunit;

namespace TrappyKeepy.Test.Service
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateUserWithCompleteSuccessShouldReturnOutcomeSuccessWithUser()
        {
            // ---------- ARRANGE ----------
            // Prepare test objects.
            var domainTestObjects = new DomainTestObjects();
            var dtoTestObjects = new DtoTestObjects();
            var userDomain = domainTestObjects.TestUser;
            var userDto = dtoTestObjects.TestUserDtoConcrete;
            // Prepare the mocks.
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var tokenServiceMock = new Mock<ITokenService>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<User>(It.IsAny<UserDto>())).Returns(userDomain);
            unitOfWorkMock.Setup(u => u.Begin()).Verifiable();
            unitOfWorkMock.Setup(u => u.users.CountByColumnValue(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(0);
            unitOfWorkMock.Setup(u => u.users.Create(userDomain)).ReturnsAsync(userDomain);
            unitOfWorkMock.Setup(u => u.Commit()).Verifiable();
            mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(userDto);
            // Instantiate the production service using the mocks.
            var userService = new UserService(
                unitOfWorkMock.Object,
                tokenServiceMock.Object,
                mapperMock.Object
            );

            // ---------- ACT ----------
            var serviceResponse = await userService.Create(new UserServiceRequest(userDto));

            // ---------- ASSERT ----------
            // ActionResult: The service returns a UserServiceResponse.
            Assert.NotNull(serviceResponse);
            Assert.IsType<UserServiceResponse>(serviceResponse);
            Assert.Null(serviceResponse.ErrorMessage);
            Assert.NotNull(serviceResponse.Outcome);
            Assert.Equal(OutcomeType.Success, serviceResponse.Outcome);
            Assert.NotNull(serviceResponse.Item);
            Assert.Equal(userDto, serviceResponse.Item);
        }
    }
}
