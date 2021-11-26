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
                command.CommandText = $"SELECT * FROM tk.filedatas_create('{filedata.KeeperId}', '{filedata.BinaryData}');";
                var result = await RunScalar(command);
                var newId = Guid.Empty;
                if (result is not null)
                {
                    newId = Guid.Parse($"{result.ToString()}");
                }
                return newId;
            }
        }

        public Task<List<Filedata>> ReadAll()
        {
            throw new NotImplementedException();
        }

        public Task<Filedata> ReadByKeeperId(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateByKeeperId(Filedata filedata)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByKeeperId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}