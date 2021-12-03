using Npgsql;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Data.Repositories
{
    public class PermitRepository : BaseRepository, IPermitRepository
    {
        public PermitRepository(NpgsqlConnection connection) : base(connection)
        {
            this.connection = connection;
        }

        public async Task<Permit> Create(Permit permit)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_create('{permit.KeeperId}'";

                if (permit.UserId is not null) command.CommandText += $", '{permit.UserId}'";
                else command.CommandText += $", null";

                if (permit.GroupId is not null) command.CommandText += $", '{permit.GroupId}'";
                else command.CommandText += $", null";

                command.CommandText += ");";

                var reader = await RunQuery(command);
                var newPermit = new Permit();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newPermit = map.Permit(reader);
                }
                reader.Close();
                return newPermit;
            }
        }

        public async Task<List<Permit>> ReadAll()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.permits_read_all();";

                var reader = await RunQuery(command);
                var permits = new List<Permit>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    permits.Add(map.Permit(reader));
                }
                reader.Close();
                return permits;
            }
        }

        public async Task<Permit> ReadById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_read_by_id('{id}');";

                var reader = await RunQuery(command);
                var permit = new Permit();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    permit = map.Permit(reader);
                }
                reader.Close();
                return permit;
            }
        }

        public async Task<List<Permit>> ReadByKeeperId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_read_by_keeper_id('{id}');";

                var reader = await RunQuery(command);
                var permits = new List<Permit>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    permits.Add(map.Permit(reader));
                }
                reader.Close();
                return permits;
            }
        }

        public async Task<List<Permit>> ReadByUserId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_read_by_user_id('{id}');";

                var reader = await RunQuery(command);
                var permits = new List<Permit>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    permits.Add(map.Permit(reader));
                }
                reader.Close();
                return permits;
            }
        }

        public async Task<List<Permit>> ReadByGroupId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_read_by_group_id('{id}');";

                var reader = await RunQuery(command);
                var permits = new List<Permit>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    permits.Add(map.Permit(reader));
                }
                reader.Close();
                return permits;
            }
        }

        public async Task<bool> DeleteById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_delete_by_id('{id}');";

                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }

        public async Task<bool> DeleteByKeeperId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_delete_by_keeper_id('{id}');";

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
                command.CommandText = $"SELECT * FROM tk.permits_delete_by_user_id('{id}');";

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
                command.CommandText = $"SELECT * FROM tk.permits_delete_by_group_id('{id}');";

                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }

        public async Task<int> PermitMatchCount(Permit permit)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permit_match_count('{permit.KeeperId}'";

                if (permit.UserId is not null) command.CommandText += $", '{permit.UserId}'";
                else command.CommandText += $", null";

                if (permit.GroupId is not null) command.CommandText += $", '{permit.GroupId}'";
                else command.CommandText += $", null";

                command.CommandText += ");";

                var result = await RunScalar(command);
                int count = 0;
                if (result is not null)
                {
                    count = int.Parse($"{result.ToString()}");
                }
                return count;
            }
        }

        public async Task<int> CountByColumnValue(string column, Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_count_by_column_value_uuid('{column}', '{id}');";
                
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
