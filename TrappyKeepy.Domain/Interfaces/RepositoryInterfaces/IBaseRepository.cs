using Npgsql;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface IBaseRepository
    {
        Task<NpgsqlDataReader> RunQuery(NpgsqlCommand command);
        Task RunNonQuery(NpgsqlCommand comamnd);
        Task<object?> RunScalar(NpgsqlCommand command);
    }
}