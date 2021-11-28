using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Test.TestObjects
{
    public class UserDtoTestObjects
    {
        public UserDto TestUserDtoReadOne
        {
            get
            {
                return new UserDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "foo",
                    Email = "foo@example.com",
                    DateCreated = DateTime.Now.AddDays(-7),
                    DateActivated = DateTime.Now.AddDays(-6),
                    DateLastLogin = DateTime.Now.AddDays(-1)
                };
            }
        }

        public List<UserDto> TestUserDtoReadAll
        {
            get
            {
                return new List<UserDto>()
                {
                    TestUserDtoReadOne,
                    TestUserDtoReadOne
                };
            }
        }
    }
}
