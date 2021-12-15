using Npgsql;
using System;
using System.IO;
using System.Threading.Tasks;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;
using TrappyKeepy.Service;

namespace TrappyKeepy.Test.End2End
{
    public class SpawnyDb
    {
        private string _testDbName;

        // Connection to use when creating the temporary testing database.
        private NpgsqlConnection _connectionCreateTestDb;

        // Connection to use when running queries in the temporary testing database.
        private NpgsqlConnection _connectionUseTestDb;
        private User? _userAdmin;
        private User? _userManager;
        private User? _userBasic;

        public SpawnyDb()
        {
            _testDbName = "keepytest";

            // Connect to the default/maintenance database named "postgres" when we create the temporary testing database.
            _connectionCreateTestDb = new NpgsqlConnection("Host=localhost;Database=postgres;Port=15432;Username=dbowner;Password=dbpass;Pooling=false");

            // Connect to the temporary testing database when we're ready to run queries like seeding test data.
            _connectionUseTestDb = new NpgsqlConnection($"Host=localhost;Database={_testDbName};Port=15432;Username=dbuser;Password=dbpass;Pooling=false");

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
            await SeedManagerUser();
            await SeedBasicUser();
            await SeedKeeperApiDll();
            await SeedKeeperDataDll();
            await SeedKeeperDomainDll();
            await SeedKeeperServiceDll();
            await SeedKeeperTestDll();
            await _connectionUseTestDb.CloseAsync();
            await _connectionUseTestDb.DisposeAsync();
        }

        public string AuthenticateAdmin()
        {
            if (_userAdmin is null)
            {
                throw new Exception("Could not encode admin user token.");
            }
            var tokenService = new TokenService();
            var token = tokenService.Encode(_userAdmin.Id, _userAdmin.Role);
            return token;
        }

        public string AuthenticateManager()
        {
            if (_userManager is null)
            {
                throw new Exception("Could not encode manager user token.");
            }
            var tokenService = new TokenService();
            var token = tokenService.Encode(_userManager.Id, _userManager.Role);
            return token;
        }

        public string AuthenticateBasic()
        {
            if (_userBasic is null)
            {
                throw new Exception("Could not encode basic user token.");
            }
            var tokenService = new TokenService();
            var token = tokenService.Encode(_userBasic.Id, _userBasic.Role);
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
                var reader = await command.ExecuteReaderAsync();
                var newUser = new User();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newUser = map.User(reader);
                }
                await reader.CloseAsync();
                _userAdmin = newUser;
            }
        }

        private async Task SeedManagerUser()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.users_create('manager', 'passwordmanager', 'manager@trappykeepy.com', 'manager');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                var reader = await command.ExecuteReaderAsync();
                var userManager = new User();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    userManager = map.User(reader);
                }
                await reader.CloseAsync();
                _userManager = userManager;
            }
        }

        private async Task SeedBasicUser()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = "SELECT * FROM tk.users_create('basic', 'passwordbasic', 'basic@trappykeepy.com', 'basic');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                var reader = await command.ExecuteReaderAsync();
                var userBasic = new User();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    userBasic = map.User(reader);
                }
                await reader.CloseAsync();
                _userBasic = userBasic;
            }
        }

        private async Task SeedKeeper(string filename)
        {
            var contentType = "application/octet-stream";
            byte[] binaryData = await File.ReadAllBytesAsync(filename);
            var adminId = Guid.Empty;
            if (_userAdmin?.Id is not null && _userAdmin.Id != Guid.Empty) adminId = _userAdmin.Id;
            var keeperId = Guid.Empty;
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_create('{filename}', '{contentType}', '{adminId}', 'The {filename} file.', 'Files');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    var keeper = map.Keeper(reader);
                    keeperId = keeper.Id;
                }
                await reader.CloseAsync();
            }
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.filedatas_create('{keeperId}', :binary_data );";
                var npgsqlParameter = new NpgsqlParameter("binary_data", NpgsqlTypes.NpgsqlDbType.Bytea);
                npgsqlParameter.Value = binaryData;
                command.Parameters.Add(npgsqlParameter);
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                await command.ExecuteScalarAsync();
            }
        }

        private async Task SeedKeeperApiDll()
        {
            await SeedKeeper("TrappyKeepy.Api.dll");
        }

        private async Task SeedKeeperDataDll()
        {
            await SeedKeeper("TrappyKeepy.Data.dll");
        }

        private async Task SeedKeeperDomainDll()
        {
            await SeedKeeper("TrappyKeepy.Domain.dll");
        }

        private async Task SeedKeeperServiceDll()
        {
            await SeedKeeper("TrappyKeepy.Service.dll");
        }

        private async Task SeedKeeperTestDll()
        {
            await SeedKeeper("TrappyKeepy.Test.dll");
        }
    }
}
