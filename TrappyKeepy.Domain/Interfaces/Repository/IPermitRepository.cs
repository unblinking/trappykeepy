using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IPermitRepository
    {
        Task<Permit> Create(Permit permit);
        Task<List<Permit>> ReadAll();
        Task<Permit> ReadById(Guid id);
        Task<List<Permit>> ReadByKeeperId(Guid id);
        Task<List<Permit>> ReadByUserId(Guid id);
        Task<List<Permit>> ReadByGroupId(Guid id);
        Task<bool> DeleteById(Guid id);
        Task<bool> DeleteByKeeperId(Guid id);
        Task<bool> DeleteByUserId(Guid id);
        Task<bool> DeleteByGroupId(Guid id);
        Task<int> PermitMatchCount(Permit permit);
        Task<int> CountByColumnValue(string column, Guid id);
    }
}
