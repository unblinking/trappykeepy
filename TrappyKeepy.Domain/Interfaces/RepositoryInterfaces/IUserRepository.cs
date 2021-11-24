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
        Task<bool> UpdateById(User user);
        Task<bool> DeleteById(Guid id);
    }
}