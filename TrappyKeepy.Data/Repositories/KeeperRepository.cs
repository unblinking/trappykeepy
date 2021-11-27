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
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_create('{keeper.Filename}'";
                if (keeper.Description is not null)
                {
                    command.CommandText += $", '{keeper.Description}'";
                }
                if (keeper.Category is not null)
                {
                    command.CommandText += $", '{keeper.Category}'";
                }
                command.CommandText += $", '{keeper.UserPosted}');";
                var result = await RunScalar(command);
                var newId = Guid.Empty;
                if (result is not null)
                {
                    newId = Guid.Parse($"{result.ToString()}");
                }
                return newId;
            }
        }

        public async Task<List<Keeper>> ReadAll()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.keepers_read_all();";
                var reader = await RunQuery(command);
                var keepers = new List<Keeper>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    keepers.Add(map.Keeper(reader));
                }
                reader.Close();
                return keepers;
            }
        }

        public async Task<Keeper> ReadById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_read_by_id('{id}');";
                var reader = await RunQuery(command);
                var keeper = new Keeper();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    keeper = map.Keeper(reader);
                }
                reader.Close();
                return keeper;
            }
        }

        public async Task<bool> UpdateById(Keeper keeper)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_update('{keeper.Id}', '{keeper.Filename}'";
                if (keeper.Description is not null)
                {
                    command.CommandText += $", '{keeper.Description}'";
                }
                if (keeper.Category is not null)
                {
                    command.CommandText += $", '{keeper.Category}'";
                }
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
                command.CommandText = $"SELECT * FROM tk.keepers_delete_by_id('{id}');";
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
                command.CommandText = $"SELECT * FROM tk.keepers_count_by_column_value_text('{column}', '{value}');";
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