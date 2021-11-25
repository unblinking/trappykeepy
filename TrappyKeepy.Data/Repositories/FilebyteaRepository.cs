using Npgsql;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public class FilebyteaRepository : BaseRepository, IFilebyteaRepository
    {
        public FilebyteaRepository(NpgsqlConnection connection) : base(connection)
        {
            this.connection = connection;
        }

        public async Task<Guid> Create(Filebytea filebytea)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Filebytea>> ReadAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Filebytea> ReadByKeeperId(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateByKeeperId(Filebytea filebytea)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteByKeeperId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}