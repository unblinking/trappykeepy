using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUserService
    {
        // Task<User> Create(User user);
        Task<List<User>> ReadAll();
        // Task<User> ReadById(Guid id);
        // Task<User> Update(User user);
        // Task<User> Delete(Guid id);
    }
}
