using Npgsql;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public abstract class BaseRepository : IBaseRepository {
        protected NpgsqlConnection connection;

        public BaseRepository(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<NpgsqlDataReader> RunQuery(NpgsqlCommand command)
        {
            command.Connection = this.connection;
            await command.PrepareAsync();
            var reader = await command.ExecuteReaderAsync();
            return reader;
        }

        public async Task RunNonQuery(NpgsqlCommand command)
        {
            command.Connection = this.connection;
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}