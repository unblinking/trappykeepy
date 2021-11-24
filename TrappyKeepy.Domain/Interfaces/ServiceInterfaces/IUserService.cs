using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserServiceResponse> Create(UserServiceRequest request);
        Task<UserServiceResponse> ReadAll(UserServiceRequest request);
        Task<UserServiceResponse> ReadById(UserServiceRequest request);
        Task<UserServiceResponse> UpdateById(UserServiceRequest request);
        Task<UserServiceResponse> UpdatePasswordById(UserServiceRequest request);
        Task<UserServiceResponse> DeleteById(UserServiceRequest request);
        Task<UserServiceResponse> Authenticate(UserServiceRequest request);
    }
}