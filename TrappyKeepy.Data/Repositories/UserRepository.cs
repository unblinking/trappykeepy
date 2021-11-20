using Npgsql;
using TrappyKeepy.Domain;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository {
        public UserRepository(NpgsqlConnection connection) : base(connection)
        {

        }

        public async Task<User> Create(User user)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = ""; // TODO: Supply the SQL that executes the stored procedure for this.
                var reader = await RunQuery(command);
                var newUser = new User();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlMap();
                    newUser = map.User(reader);
                }
                reader.Close();
                return newUser;
            }
        }

        public async Task<List<User>> ReadAll()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.users_ReadAll()";
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
                command.CommandText = ""; // TODO: Supply the SQL that executes the stored procedure for this.
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
                command.CommandText = ""; // TODO: Supply the SQL that executes the stored procedure for this.
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
                command.CommandText = ""; // TODO: Supply the SQL that executes the stored procedure for this.
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
    }
}