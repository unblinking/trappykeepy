using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IKeeperService
    {
        Task<KeeperServiceResponse> Create(KeeperServiceRequest request);
        Task<KeeperServiceResponse> ReadAll(KeeperServiceRequest request);
        Task<KeeperServiceResponse> ReadById(KeeperServiceRequest request);
        Task<KeeperServiceResponse> UpdateById(KeeperServiceRequest request);
        Task<KeeperServiceResponse> DeleteById(KeeperServiceRequest request);
    }
}