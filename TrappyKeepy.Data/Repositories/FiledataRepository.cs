using Npgsql;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public class FiledataRepository : BaseRepository, IFiledataRepository
    {
        public FiledataRepository(NpgsqlConnection connection) : base(connection)
        {
            this.connection = connection;
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

        /**
         *
         * I don't think anyone would ever really want to do this.
         * but there is a function written for it, commented out.
         *
         */
        public Task<List<Filedata>> ReadAll()
        {
            throw new NotImplementedException();
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

        /**
         * Not going to ahave a function to update a filedatas record. If you want to do
         * that, delete the old keeper/filedata and then insert a new record set.
         */
        public Task<bool> UpdateByKeeperId(Filedata filedata)
        {
            throw new NotImplementedException();
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