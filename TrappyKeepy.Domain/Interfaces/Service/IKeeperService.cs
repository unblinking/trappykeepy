using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IKeeperService
    {
        Task<IKeeperServiceResponse> Create(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> ReadAll(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> ReadById(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> UpdateById(IKeeperServiceRequest request);
        Task<IKeeperServiceResponse> DeleteById(IKeeperServiceRequest request);
    }
}
