using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IKeeperRepository : IBaseRepository
    {
        Task<Guid> Create(Keeper keeper);
        Task<List<Keeper>> ReadAll();
        Task<Keeper> ReadById(Guid id);
        Task<bool> UpdateById(Keeper keeper);
        Task<bool> DeleteById(Guid id);
        Task<int> CountByColumnValue(string column, string value);
    }
}