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

        public Task<List<Keeper>> ReadAll()
        {
            throw new NotImplementedException();
        }

        public Task<Keeper> ReadById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateById(Keeper keeper)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteById(Guid id)
        {
            throw new NotImplementedException();
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