using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IMembershipRepository
    {
        Task<Guid> Create(Membership membership);
        Task<List<Membership>> ReadAll();
        Task<Membership> ReadByGroupId(Guid id);
        Task<Membership> ReadByUserId(Guid id);
        Task<bool> DeleteByGroupId(Guid id);
        Task<bool> DeleteByUserId(Guid id);
    }
}
