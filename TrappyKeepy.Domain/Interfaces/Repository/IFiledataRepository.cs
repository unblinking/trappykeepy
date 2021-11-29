using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IFiledataRepository : IBaseRepository
    {
        Task<Guid> Create(Filedata filedata);
        Task<Filedata> ReadByKeeperId(Guid keeper_id);
        Task<bool> DeleteByKeeperId(Guid keeper_id);
    }
}
