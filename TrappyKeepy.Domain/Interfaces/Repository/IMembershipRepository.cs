using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IMembershipRepository
    {
        Task<Membership> Create(Membership membership);
        Task<List<Membership>> ReadAll();
        Task<Membership> ReadById(Guid id);
        Task<List<Membership>> ReadByGroupId(Guid id);
        Task<List<Membership>> ReadByUserId(Guid id);
        Task<bool> DeleteById(Guid id);
        Task<bool> DeleteByGroupId(Guid id);
        Task<bool> DeleteByUserId(Guid id);
        Task<int> CountByColumnValue(string column, Guid id);
        Task<int> CountByGroupAndUser(Guid groupId, Guid userId);
    }
}
