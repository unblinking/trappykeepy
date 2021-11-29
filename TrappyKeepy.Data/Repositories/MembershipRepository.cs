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

        public async Task<Guid> Create(Membership membership)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_create('{membership.GroupId}', '{membership.UserId}');";
                var result = await RunScalar(command);
                var newId = Guid.Empty;
                if (result is not null)
                {
                    newId = Guid.Parse($"{result.ToString()}");
                }
                return newId;
            }
        }

        public async Task<List<Membership>> ReadAll()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.memberships_read_all();";
                var reader = await RunQuery(command);
                var memberships = new List<Membership>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    memberships.Add(map.Membership(reader));
                }
                reader.Close();
                return memberships;
            }
        }

        public async Task<Membership> ReadByGroupId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_read_by_group_id('{id}');";
                var reader = await RunQuery(command);
                var membership = new Membership();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    membership = map.Membership(reader);
                }
                reader.Close();
                return membership;
            }
        }

        public async Task<Membership> ReadByUserId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_read_by_user_id('{id}');";
                var reader = await RunQuery(command);
                var membership = new Membership();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    membership = map.Membership(reader);
                }
                reader.Close();
                return membership;
            }
        }

        public async Task<bool> DeleteByGroupId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_delete_by_group_id('{id}');";
                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }

        public async Task<bool> DeleteByUserId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_delete_by_user_id('{id}');";
                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }
    }
}
