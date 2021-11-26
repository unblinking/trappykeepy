using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        Task<Guid> Create(User user);
        Task<List<User>> ReadAll();
        Task<User> ReadById(Guid id);
        Task<bool> UpdateById(User user);
        Task<bool> UpdatePasswordById(User user);
        Task<bool> DeleteById(Guid id);
        Task<int> CountByColumnValue(string column, string value);
        Task<Guid> Authenticate(User user);
    }
}