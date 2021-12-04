using Npgsql;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public abstract class BaseRepository : IBaseRepository
    {
        protected NpgsqlConnection _connection;

        public BaseRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<NpgsqlDataReader> RunQuery(NpgsqlCommand command)
        {
            command.Connection = _connection;
            await command.PrepareAsync();
            var reader = await command.ExecuteReaderAsync();
            return reader;
        }

        public async Task RunNonQuery(NpgsqlCommand command)
        {
            command.Connection = _connection;
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<object?> RunScalar(NpgsqlCommand command)
        {
            command.Connection = _connection;
            object? result = await command.ExecuteScalarAsync();
            return result;
        }
    }
}
