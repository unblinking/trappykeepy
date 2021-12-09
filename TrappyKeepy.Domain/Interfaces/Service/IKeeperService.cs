using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IKeeperService
    {
        Task<IKeeperServiceResponse> Create(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> ReadAllPermitted(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> ReadByIdPermitted(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> Update(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> DeleteById(IKeeperServiceRequest request);
    }
}
