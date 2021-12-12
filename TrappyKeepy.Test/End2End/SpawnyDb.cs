using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrappyKeepy.Test.End2End
{
    public class SpawnyDb
    {
        private string _testDbName;

        // Connection to use when creating the temporary testing database.
        private NpgsqlConnection _connectionCreateTestDb;

        // Connection to use when running queries in the temporary testing database.
        private NpgsqlConnection _connectionUseTestDb;

        public SpawnyDb()
        {
            _testDbName = "keepytest";

            // Connect to the default/maintenance database named "postgres" when we create the temporary testing database.
            _connectionCreateTestDb = new NpgsqlConnection("Host=localhost;Database=postgres;Port=15432;Username=dbowner;Password=dbpass;Pooling=false");

            // Connect to the temporary testing database when we're ready to run queries like seeding test data.
            _connectionUseTestDb = new NpgsqlConnection($"Host=localhost;Database={_testDbName};Port=15432;Username=dbowner;Password=dbpass;Pooling=false");

            // Set the TKDB_CONN_STRING env var that the UnitOfWork class will use to connect to the database.
            // This way when the WebApplicationFactory creates the API in memory for the e2e tests, the UnitOfWork
            // class will connect to the temporary testing database instead of the development database.
            Environment.SetEnvironmentVariable("TKDB_CONN_STRING", $"Host=localhost;Database={_testDbName};Port=15432;Username=dbowner;Password=dbpass;Pooling=false");
        }

        public async Task RecycleDb()
        {
            // Drop any old temporary testing database, and create a fresh one.
            await _connectionCreateTestDb.OpenAsync();
            await Drop();
            await Create();
            await _connectionCreateTestDb.CloseAsync();
            await _connectionCreateTestDb.DisposeAsync();

            // Seed test data into the temporary testing database for the next e2e test.
            await _connectionUseTestDb.OpenAsync();
            await SeedAdminUser();
            await _connectionUseTestDb.CloseAsync();
            await _connectionUseTestDb.DisposeAsync();
        }

        private async Task Drop()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"DROP DATABASE IF EXISTS {_testDbName};";
                command.Connection = _connectionCreateTestDb;
                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task Create()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"CREATE DATABASE {_testDbName} WITH TEMPLATE keepydb OWNER dbowner;";
                command.Connection = _connectionCreateTestDb;
                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task SeedAdminUser()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.users_create('foo', 'passwordfoo', 'foo@trappykeepy.com', 'admin');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                await command.ExecuteReaderAsync();
            }
        }
    }
}
