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
            _connection = connection;
        }

        public async Task<Membership> Create(Membership membership)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_create('{membership.GroupId}', '{membership.UserId}');";

                var reader = await RunQuery(command);
                var newMembership = new Membership();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newMembership = map.Membership(reader);
                }
                reader.Close();
                return newMembership;
            }
        }

        public async Task<Membership> ReadById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_read_by_id('{id}');";

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

        public async Task<List<Membership>> ReadByGroupId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_read_by_group_id('{id}');";
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

        public async Task<List<Membership>> ReadByUserId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_read_by_user_id('{id}');";
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

        public async Task<bool> DeleteById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_delete_by_id('{id}');";
                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
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

        public async Task<int> CountByColumnValue(string column, Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_count_by_column_value_uuid('{column}', '{id}');";
                var result = await RunScalar(command);
                int count = 0;
                if (result is not null)
                {
                    count = int.Parse($"{result.ToString()}");
                }
                return count;
            }
        }

        public async Task<int> CountByGroupAndUser(Guid groupId, Guid userId)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_count_by_group_and_user('{groupId}', '{userId}');";
                var result = await RunScalar(command);
                int count = 0;
                if (result is not null)
                {
                    count = int.Parse($"{result.ToString()}");
                }
                return count;
            }
        }
    }
}
