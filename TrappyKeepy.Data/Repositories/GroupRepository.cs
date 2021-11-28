using Npgsql;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Data.Repositories
{
    public class GroupRepository : BaseRepository, IGroupRepository
    {
        public GroupRepository(NpgsqlConnection connection) : base(connection)
        {
            this.connection = connection;
        }


    }
}
