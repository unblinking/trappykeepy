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
                    Name = "foo",
                    Password = "passwordfoo",
                    Email = "foo@trappykeepy.com",
                    DateCreated = DateTime.Now.AddDays(-7),
                    DateActivated = DateTime.Now.AddDays(-6),
                    DateLastLogin = DateTime.Now.AddDays(-1)
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
