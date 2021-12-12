using System;
using System.Collections.Generic;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Test.TestObjects
{
    public class DtoTestObjects
    {
        public IUserDto TestUserDto
        {
            get
            {
                return new UserDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "basic",
                    Password = "passwordbasic",
                    Email = "basic@trappykeepy.com",
                    Role = "basic",
                    DateCreated = DateTime.Now.AddDays(-7),
                    DateActivated = DateTime.Now.AddDays(-6),
                    DateLastLogin = DateTime.Now.AddDays(-1)
                };
            }
        }

        public IUserDto TestUserDtoIncomplete
        {
            get
            {
                return new UserDto()
                {
                    Name = "incomplete",
                };
            }
        }

        public IUserDto TestUserNewBasic
        {
            get
            {
                return new UserDto()
                {
                    Name = "newbasic",
                    Password = "passwordnewbasic",
                    Email = "newbasic@trappykeepy.com",
                    Role = "basic"
                };
            }
        }

        public List<IUserDto> TestUserDtoList
        {
            get
            {
                return new List<IUserDto>()
                {
                    TestUserDto,
                    TestUserDto
                };
            }
        }

        public IUserSessionDto TestUserSessionDto
        {
            get
            {
                return new UserSessionDto("foo@trappykeepy.com", "passwordfoo");
            }
        }

        public IUserSessionDto TestUserSessionAdminDto
        {
            get
            {
                return new UserSessionDto("admin@trappykeepy.com", "passwordadmin");
            }
        }

        public IMembershipDto TestMembershipDto
        {
            get
            {
                return new MembershipDto()
                {
                    Id = Guid.NewGuid(),
                    GroupId = Guid.NewGuid(),
                    UserId = Guid.NewGuid()
                };
            }
        }

        public IPermitDto TestPermitDto
        {
            get
            {
                return new PermitDto()
                {
                    KeeperId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    GroupId = Guid.NewGuid()
                };
            }
        }
    }
}
