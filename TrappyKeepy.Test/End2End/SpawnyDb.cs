using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Test.TestObjects;

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
            Environment.SetEnvironmentVariable("TKDB_CONN_STRING", $"Host=localhost;Database={_testDbName};Port=15432;Username=dbuser;Password=dbpass;Pooling=false");
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

        public async Task<string> AuthenticateAdmin(HttpClient client)
        {
            var dto = new DtoTestObjects();
            var user = dto.TestUserSessionAdminDto;
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/v1/sessions", content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var controllerResponse = JsonSerializer.Deserialize<ControllerResponse>(responseJson, jsonOpts);
            if (controllerResponse is null || controllerResponse.Data is null)
            {
                throw new Exception("Could not authenticate for e2e tests. No controller response data.");
            }
            var token = controllerResponse.Data.ToString();
            if (token is null)
            {
                throw new Exception("Could not authenticate for e2e tests. No token.");
            }
            return token;
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
                command.CommandText = "SELECT * FROM tk.users_create('admin', 'passwordadmin', 'admin@trappykeepy.com', 'admin');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                await command.ExecuteReaderAsync();
            }
        }
    }
}
