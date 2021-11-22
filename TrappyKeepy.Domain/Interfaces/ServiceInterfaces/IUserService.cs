using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserServiceResponse> Create(UserServiceRequest request);
        Task<UserServiceResponse> ReadAll(UserServiceRequest request);
        // Task<User> ReadById(Guid id);
        // Task<User> Update(User user);
        // Task<User> Delete(Guid id);
    }
}
