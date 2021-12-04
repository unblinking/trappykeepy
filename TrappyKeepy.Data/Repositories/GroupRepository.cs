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
            _connection = connection;
        }

        public async Task<Group> Create(Group group)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.groups_create('{group.Name}'";

                if (group.Description is not null) command.CommandText += $", '{group.Description}'";
                else command.CommandText += $", null";

                command.CommandText += ");";

                var reader = await RunQuery(command);
                var newGroup = new Group();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newGroup = map.Group(reader);
                }
                reader.Close();
                return newGroup;
            }
        }

        public async Task<List<Group>> ReadAll()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.groups_read_all();";

                var reader = await RunQuery(command);
                var groups = new List<Group>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    groups.Add(map.Group(reader));
                }
                reader.Close();
                return groups;
            }
        }

        public async Task<Group> ReadById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.groups_read_by_id('{id}');";

                var reader = await RunQuery(command);
                var group = new Group();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    group = map.Group(reader);
                }
                reader.Close();
                return group;
            }
        }

        public async Task<bool> UpdateById(Group group)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.groups_update('{group.Id}'";

                if (group.Name is not null) command.CommandText += $", '{group.Name}'";
                else command.CommandText += $", null";

                if (group.Description is not null) command.CommandText += $", '{group.Description}'";
                else command.CommandText += $", null";

                command.CommandText += ");";

                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }

        public async Task<bool> DeleteById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.groups_delete_by_id('{id}');";

                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }

        public async Task<int> CountByColumnValue(string column, string value)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.groups_count_by_column_value_text('{column}', '{value}');";

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
