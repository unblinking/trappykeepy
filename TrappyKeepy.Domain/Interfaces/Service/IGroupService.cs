using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IGroupService
    {
        Task<IGroupServiceResponse> Create(IGroupServiceRequest request);
        Task<IGroupServiceResponse> ReadAll(IGroupServiceRequest request);
        Task<IGroupServiceResponse> ReadById(IGroupServiceRequest request);
        Task<IGroupServiceResponse> UpdateById(IGroupServiceRequest request);
        Task<IGroupServiceResponse> DeleteById(IGroupServiceRequest request);
    }
}
