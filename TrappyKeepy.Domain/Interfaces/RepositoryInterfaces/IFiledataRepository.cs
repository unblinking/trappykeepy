using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IFiledataRepository : IBaseRepository
    {
        Task<Guid> Create(Filedata filedata);
        Task<List<Filedata>> ReadAll();
        Task<Filedata> ReadByKeeperId(Guid keeper_id);
        Task<bool> UpdateByKeeperId(Filedata filedata);
        Task<bool> DeleteByKeeperId(Guid keeper_id);
    }
}
