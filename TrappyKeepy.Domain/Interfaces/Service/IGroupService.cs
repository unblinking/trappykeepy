using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IGroupService
    {
        Task<GroupServiceResponse> Create(GroupServiceRequest request);
        Task<GroupServiceResponse> ReadAll(GroupServiceRequest request);
        Task<GroupServiceResponse> ReadById(GroupServiceRequest request);
        Task<GroupServiceResponse> UpdateById(GroupServiceRequest request);
        Task<GroupServiceResponse> DeleteById(GroupServiceRequest request);
    }
}
