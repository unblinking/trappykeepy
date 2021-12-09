using System.ComponentModel;
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
        private DomainTestObjects _domain;
        private DtoTestObjects _dto;
        private Mock<IUnitOfWork> _uow;
        private Mock<ITokenService> _token;
        private Mock<IMapper> _mapper;

        public UserServiceTests()
        {
            _domain = new DomainTestObjects();
            _dto = new DtoTestObjects();
            _uow = new Mock<IUnitOfWork>();
            _token = new Mock<ITokenService>();
            _mapper = new Mock<IMapper>();
        }

        public void RefreshMocks()
        {
            _uow = new Mock<IUnitOfWork>();
            _token = new Mock<ITokenService>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task CreateUserWithCompleteSuccessShouldReturnOutcomeSuccessWithUser()
        {
            // ---------- ARRANGE ----------
            RefreshMocks();
            var userDomain = _domain.TestUser;
            var userDto = _dto.TestUserDto;
            _mapper.Setup(m => m.Map<User>(It.IsAny<UserDto>())).Returns(userDomain);
            _uow.Setup(u => u.Begin()).Verifiable();
            _uow.Setup(u => u.users.CountByColumnValue(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(0);
            _uow.Setup(u => u.users.Create(userDomain)).ReturnsAsync(userDomain);
            _uow.Setup(u => u.Commit()).Verifiable();
            _mapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns((UserDto)userDto);
            var userService = new UserService(_uow.Object, _token.Object, _mapper.Object);

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
