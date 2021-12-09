using System;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Test.TestObjects
{
    public class DomainTestObjects
    {
        public User TestUser
        {
            get
            {
                return new User()
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
    }
}
