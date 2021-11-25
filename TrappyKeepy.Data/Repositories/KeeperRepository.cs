using Npgsql;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public class KeeperRepository : BaseRepository, IKeeperRepository
    {
        public KeeperRepository(NpgsqlConnection connection) : base(connection)
        {
            this.connection = connection;
        }

        public async Task<Guid> Create(Keeper keeper)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Keeper>> ReadAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Keeper> ReadById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateById(Keeper keeper)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}