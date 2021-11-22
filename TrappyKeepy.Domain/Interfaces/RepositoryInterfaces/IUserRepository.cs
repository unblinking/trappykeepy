using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        Task<Guid> Create(User user);
        Task<List<User>> ReadAll();
        Task<User> ReadById(Guid id);
        Task<User> Update(User user);
        Task<bool> Delete(Guid id);
    }
}