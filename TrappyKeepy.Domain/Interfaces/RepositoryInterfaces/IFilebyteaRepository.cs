using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IFilebyteaRepository : IBaseRepository
    {
        Task<Guid> Create(Filebytea filebytea);
        Task<List<Filebytea>> ReadAll();
        Task<Filebytea> ReadByKeeperId(Guid keeper_id);
        Task<bool> UpdateByKeeperId(Filebytea filebytea);
        Task<bool> DeleteByKeeperId(Guid keeper_id);
    }
}
