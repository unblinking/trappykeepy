using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IMembershipRepository
    {
        Task<Guid> Create(Membership membership);
        Task<List<Membership>> ReadAll();
        Task<List<Membership>> ReadByGroupId(Guid id);
        Task<List<Membership>> ReadByUserId(Guid id);
        Task<bool> DeleteByGroupId(Guid id);
        Task<bool> DeleteByUserId(Guid id);
        Task<bool> DeleteByGroupIdAndUserId(Guid groupId, Guid userId);
        Task<int> CountByColumnValue(string column, Guid id);
        Task<int> CountByGroupAndUser(Guid groupId, Guid userId);
    }
}
