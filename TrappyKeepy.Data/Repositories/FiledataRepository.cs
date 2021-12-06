using Npgsql;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Data.Repositories
{
    public class FiledataRepository : BaseRepository, IFiledataRepository
    {
        public FiledataRepository(NpgsqlConnection connection) : base(connection)
        {
            _connection = connection;
        }

        public async Task<Guid> Create(Filedata filedata)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.filedatas_create('{filedata.KeeperId}', :binary_data );";

                var npgsqlParameter = new NpgsqlParameter("binary_data", NpgsqlTypes.NpgsqlDbType.Bytea);
                npgsqlParameter.Value = filedata.BinaryData;

                command.Parameters.Add(npgsqlParameter);

                var result = await RunScalar(command);
                var newId = Guid.Empty;
                if (result is not null)
                {
                    newId = Guid.Parse($"{result.ToString()}");
                }
                return newId;
            }
        }

        public async Task<Filedata> ReadByKeeperId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.filedatas_read_by_keeper_id('{id}');";

                var reader = await RunQuery(command);
                var filedata = new Filedata();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    filedata = map.Filedata(reader);
                }
                reader.Close();
                return filedata;
            }
        }

        public async Task<bool> DeleteByKeeperId(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.filedatas_delete_by_keeper_id('{id}');";

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
