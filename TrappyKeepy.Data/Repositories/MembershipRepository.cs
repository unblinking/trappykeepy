using Npgsql;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Data.Repositories
{
    public class MembershipRepository : BaseRepository, IMembershipRepository
    {
        public MembershipRepository(NpgsqlConnection connection) : base(connection)
        {
            this.connection = connection;
        }


    }
}
