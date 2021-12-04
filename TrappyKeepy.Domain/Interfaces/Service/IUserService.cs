using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IUserService
    {
        Task<IUserServiceResponse> Create(IUserServiceRequest request);
        Task<IUserServiceResponse> ReadAll(IUserServiceRequest request);
        Task<IUserServiceResponse> ReadById(IUserServiceRequest request);
        Task<IUserServiceResponse> Update(IUserServiceRequest request);
        Task<IUserServiceResponse> UpdatePassword(IUserServiceRequest request);
        Task<IUserServiceResponse> DeleteById(IUserServiceRequest request);
        Task<IUserServiceResponse> CreateSession(IUserServiceRequest request);
    }
}
