using System;
using System.Collections.Generic;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Test.TestObjects
{
    public class DtoTestObjects
    {
        public IUserDto TestUserAdminDto
        {
            get
            {
                return new UserDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "admin",
                    Password = "passwordadmin",
                    Email = "admin@trappykeepy.com",
                    Role = "admin",
                    DateCreated = DateTime.Now.AddDays(-7),
                    DateActivated = DateTime.Now.AddDays(-6),
                    DateLastLogin = DateTime.Now.AddDays(-1)
                };
            }
        }

        public IUserDto TestUserManagerDto
        {
            get
            {
                return new UserDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "manager",
                    Password = "passwordmanager",
                    Email = "manager@trappykeepy.com",
                    Role = "manager",
                    DateCreated = DateTime.Now.AddDays(-7),
                    DateActivated = DateTime.Now.AddDays(-6),
                    DateLastLogin = DateTime.Now.AddDays(-1)
                };
            }
        }

        public IUserDto TestUserBasicDto
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

        public IUserDto TestUserIncompleteDto
        {
            get
            {
                return new UserDto()
                {
                    Name = "incomplete",
                };
            }
        }

        public IUserDto TestUserEmptyStringsDto
        {
            get
            {
                return new UserDto()
                {
                    Name = "",
                    Password = "",
                    Email = "",
                    Role = ""
                };
            }
        }

        public IUserDto TestUserNewBasicDto
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
                    TestUserBasicDto,
                    TestUserManagerDto,
                    TestUserAdminDto
                };
            }
        }

        public IUserSessionDto TestUserSessionAdminDto
        {
            get
            {
                return new UserSessionDto("admin@trappykeepy.com", "passwordadmin");
            }
        }

        public IUserSessionDto TestUserSessionManagerDto
        {
            get
            {
                return new UserSessionDto("manager@trappykeepy.com", "passwordmanager");
            }
        }

        public IUserSessionDto TestUserSessionBasicDto
        {
            get
            {
                return new UserSessionDto("basic@trappykeepy.com", "passwordbasic");
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

        public IGroupDto TestGroupNewDto
        {
            get
            {
                return new GroupDto()
                {
                    Name = "New Group",
                    Description = "A new group for users to join.",
                };
            }
        }
    }
}
