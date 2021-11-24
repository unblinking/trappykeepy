using Npgsql;
using TrappyKeepy.Domain;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(NpgsqlConnection connection) : base(connection)
        {
            this.connection = connection;
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

        public async Task<bool> UpdateById(User user)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.users_update('{user.Id}', '{user.Name}', '{user.Email}'";
                if (user.DateActivated is not null)
                {
                    command.CommandText += $", '{user.DateActivated}'";
                }
                if (user.DateLastLogin is not null)
                {
                    command.CommandText += $", '{user.DateLastLogin}'";
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

        public async Task<bool> UpdatePasswordById(User user)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.users_update_password('{user.Id}', '{user.Password}');";
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
                // TODO: Supply the SQL that executes the stored procedure for this.
                command.CommandText = $"SELECT * FROM tk.users_delete_by_id('{id}');";
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

        public async Task<Guid> Authenticate(User user)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.users_authenticate('{user.Email}', '{user.Password}');";
                var result = await RunScalar(command);
                var authenticatedId = Guid.Empty;
                if (result is not null)
                {
                    authenticatedId = Guid.Parse($"{result.ToString()}");
                }
                return authenticatedId;
            }
        }
    }
}