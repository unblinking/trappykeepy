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

        public User TestManagerUser
        {
            get
            {
                return new User()
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

        public User TestAdminUser
        {
            get
            {
                return new User()
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
    }
}
