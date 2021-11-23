using Npgsql;
using TrappyKeepy.Domain;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(NpgsqlConnection connection) : base(connection)
        {

        }

        public async Task<Guid> Create(User user)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.users_insert('{user.Name}', '{user.Password}', '{user.Email}', '{user.DateCreated}');";
                var result = await RunScalar(command);
                var newId = Guid.Empty;
                if (result is not null)
                {
                    newId = Guid.Parse($"{result.ToString()}");
                }
                return newId;
            }
        }

        public async Task<List<User>> ReadAll()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.users_read_all();";
                var reader = await RunQuery(command);
                var users = new List<User>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlMap();
                    users.Add(map.User(reader));
                }
                reader.Close();
                return users;
            }
        }

        public async Task<User> ReadById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.users_read_by_id('{id}');";
                var reader = await RunQuery(command);
                var user = new User();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlMap();
                    user = map.User(reader);
                }
                reader.Close();
                return user;
            }
        }

        public async Task<User> Update(User user)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $""; // TODO: Supply the SQL that executes the stored procedure for this.
                var reader = await RunQuery(command);
                var updatedUser = new User();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlMap();
                    updatedUser = map.User(reader);
                }
                reader.Close();
                return updatedUser;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $""; // TODO: Supply the SQL that executes the stored procedure for this.
                var reader = await RunQuery(command);
                var success = false;
                while (await reader.ReadAsync())
                {
                    // TODO: Read the result to determine if success or not.
                }
                reader.Close();
                return success;
            }
        }

        public async Task<int> CountByColumnValue(string column, string value)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.users_count_by_column_value_text('{column}', '{value}');";
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