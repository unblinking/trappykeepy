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

        // Seeded users in the temporary testing database.
        // Can be handy to keep track of them here (e.g. we can issue authentication tokens for their user id).
        // Keep in mind these are re-seeded/recycled/replaced with fresh users between every test.
        private User? _userAdmin;
        private User? _userManager;
        private User? _userBasic;

        // Seeded keepers in the temporary testing database.
        private Keeper? _keeperApiDll;
        private Keeper? _keeperDataDll;
        private Keeper? _keeperDomainDll;
        private Keeper? _keeperServiceDll;
        private Keeper? _keeperTestDll;

        // Seeded groups in the temporary testing database.
        private Group? _groupAdmins;
        private Group? _groupManagers;
        private Group? _groupBasics;
        private Group? _groupLaughingstocks;

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
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        }

        public async Task RecycleDb()
        {
            // Drop any old temporary testing database, and create a fresh one.
            await _connectionCreateTestDb.OpenAsync();
            await DropDb();
            await CreateDb();
            await _connectionCreateTestDb.CloseAsync();
            await _connectionCreateTestDb.DisposeAsync();

            // Seed test data into the temporary testing database for the next e2e test.
            await _connectionUseTestDb.OpenAsync();
            await SeedUserAdmin();
            await SeedUserManager();
            await SeedUserBasic();
            await SeedKeeperApiDll();
            await SeedKeeperDataDll();
            await SeedKeeperDomainDll();
            await SeedKeeperServiceDll();
            await SeedKeeperTestDll();
            await SeedGroupAdmins();
            await SeedGroupManagers();
            await SeedGroupBasics();
            await SeedGroupLaughingstocks();
            await SeedMembershipAdmin();
            await SeedMembershipManager();
            await SeedMembershipBasic();
            await SeedPermitApiDllAdminUser();
            await SeedPermitDataDllAdminsGroup();
            await SeedPermitDomainDllManagerUser();
            await SeedPermitServiceDllManagersGroup();
            await SeedPermitTestDllBasicUser();
            await _connectionUseTestDb.CloseAsync();
            await _connectionUseTestDb.DisposeAsync();
        }

        private async Task DropDb()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"DROP DATABASE IF EXISTS {_testDbName};";
                command.Connection = _connectionCreateTestDb;
                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateDb()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"CREATE DATABASE {_testDbName} WITH TEMPLATE keepydb OWNER dbowner;";
                command.Connection = _connectionCreateTestDb;
                await command.PrepareAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        #region PUBLIC METHODS

        public User GetUserAdmin()
        {
            if (_userAdmin is null) throw new Exception("Could not return admin user.");
            return _userAdmin;
        }

        public User GetUserManager()
        {
            if (_userManager is null) throw new Exception("Could not return manager user.");
            return _userManager;
        }

        public User GetUserBasic()
        {
            if (_userBasic is null) throw new Exception("Could not return basic user.");
            return _userBasic;
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

        public Keeper GetKeeperApiDll()
        {
            if (_keeperApiDll is null) throw new Exception("Could not return Api dll keeper");
            return _keeperApiDll;
        }

        public Keeper GetKeeperDataDll()
        {
            if (_keeperDataDll is null) throw new Exception("Could not return Data dll keeper");
            return _keeperDataDll;
        }

        public Keeper GetKeeperDominaDll()
        {
            if (_keeperDomainDll is null) throw new Exception("Could not return Domain dll keeper");
            return _keeperDomainDll;
        }

        public Keeper GetKeeperServiceDll()
        {
            if (_keeperServiceDll is null) throw new Exception("Could not return Service dll keeper");
            return _keeperServiceDll;
        }

        public Keeper GetKeeperTestDll()
        {
            if (_keeperTestDll is null) throw new Exception("Could not return Test dll keeper");
            return _keeperTestDll;
        }

        public Group GetGroupAdmins()
        {
            if (_groupAdmins is null) throw new Exception("Could not return group Admins.");
            return _groupAdmins;
        }

        public Group GetGroupManagers()
        {
            if (_groupManagers is null) throw new Exception("Could not return group Managers.");
            return _groupManagers;
        }

        public Group GetGroupBasics()
        {
            if (_groupBasics is null) throw new Exception("Could not return group Basics.");
            return _groupBasics;
        }

        public Group GetGroupLaughingstocks()
        {
            if (_groupLaughingstocks is null) throw new Exception("Could not return group Laughingstocks.");
            return _groupLaughingstocks;
        }

        #endregion PUBLIC METHODS

        #region USERS

        private async Task<User> SeedUser(string name, string password, string email, string role)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.users_create('{name}', '{password}', '{email}', '{role}');";
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
                return newUser;
            }
        }

        private async Task SeedUserAdmin()
        {
            _userAdmin = await SeedUser("admin", "passwordadmin", "admin@trappykeepy.com", "admin");
        }

        private async Task SeedUserManager()
        {
            _userManager = await SeedUser("manager", "passwordmanager", "manager@trappykeepy.com", "manager");
        }

        private async Task SeedUserBasic()
        {
            _userBasic = await SeedUser("basic", "passwordbasic", "basic@trappykeepy.com", "basic");
        }

        #endregion USERS

        #region KEEPERS

        private async Task<Keeper> SeedKeeper(string filename)
        {
            var contentType = "application/octet-stream";
            byte[] binaryData = await File.ReadAllBytesAsync(filename);
            var adminId = Guid.Empty;
            if (_userAdmin?.Id is not null && _userAdmin.Id != Guid.Empty) adminId = _userAdmin.Id;
            Keeper keeper = new Keeper();
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_create('{filename}', '{contentType}', '{adminId}', 'The {filename} file.', 'Files');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    keeper = map.Keeper(reader);
                }
                await reader.CloseAsync();
            }
            if (keeper.Id == Guid.Empty) throw new Exception("Could not seed keeper.");
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.filedatas_create('{keeper.Id}', :binary_data );";
                var npgsqlParameter = new NpgsqlParameter("binary_data", NpgsqlTypes.NpgsqlDbType.Bytea);
                npgsqlParameter.Value = binaryData;
                command.Parameters.Add(npgsqlParameter);
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                await command.ExecuteScalarAsync();
            }
            return keeper;
        }

        private async Task SeedKeeperApiDll()
        {
            _keeperApiDll = await SeedKeeper("TrappyKeepy.Api.dll");
        }

        private async Task SeedKeeperDataDll()
        {
            _keeperDataDll = await SeedKeeper("TrappyKeepy.Data.dll");
        }

        private async Task SeedKeeperDomainDll()
        {
            _keeperDomainDll = await SeedKeeper("TrappyKeepy.Domain.dll");
        }

        private async Task SeedKeeperServiceDll()
        {
            _keeperServiceDll = await SeedKeeper("TrappyKeepy.Service.dll");
        }

        private async Task SeedKeeperTestDll()
        {
            _keeperTestDll = await SeedKeeper("TrappyKeepy.Test.dll");
        }

        #endregion KEEPERS

        #region GROUPS

        private async Task<Group> SeedGroup(string name, string description)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.groups_create('{name}', '{description}');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                var reader = await command.ExecuteReaderAsync();
                var newGroup = new Group();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newGroup = map.Group(reader);
                }
                await reader.CloseAsync();
                return newGroup;
            }
        }

        private async Task SeedGroupAdmins()
        {
            _groupAdmins = await SeedGroup("Admins", "Group for all administrator users.");
        }

        private async Task SeedGroupManagers()
        {
            _groupManagers = await SeedGroup("Managers", "Group for all manager users.");
        }

        private async Task SeedGroupBasics()
        {
            _groupBasics = await SeedGroup("Basics", "Group for all basic users.");
        }

        private async Task SeedGroupLaughingstocks()
        {
            _groupLaughingstocks = await SeedGroup("Laughingstocks", "The infamous nothingworksright laughingstocks.");
        }

        #endregion GROUPS

        #region MEMBERSHIPS

        private async Task<Membership> SeedMembership(Guid groupId, Guid userId)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.memberships_create('{groupId}', '{userId}');";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                var reader = await command.ExecuteReaderAsync();
                var newMembership = new Membership();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newMembership = map.Membership(reader);
                }
                await reader.CloseAsync();
                return newMembership;
            }
        }

        private async Task SeedMembershipAdmin()
        {
            if (_groupAdmins is null || _userAdmin is null)
            {
                throw new Exception("Could not seed membership.");
            }
            await SeedMembership(_groupAdmins.Id, _userAdmin.Id);
        }

        private async Task SeedMembershipManager()
        {
            if (_groupManagers is null || _userManager is null)
            {
                throw new Exception("Could not seed membership.");
            }
            await SeedMembership(_groupManagers.Id, _userManager.Id);
        }

        private async Task SeedMembershipBasic()
        {
            if (_groupBasics is null || _userBasic is null)
            {
                throw new Exception("Could not seed membership.");
            }
            await SeedMembership(_groupBasics.Id, _userBasic.Id);
        }

        #endregion MEMBERSHIPS

        #region PERMITS

        private async Task<Permit> SeedPermit(Guid keeperId, Guid? userId, Guid? groupId)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.permits_create('{keeperId}'";

                if (userId is not null) command.CommandText += $", '{userId}'";
                else command.CommandText += $", null";

                if (groupId is not null) command.CommandText += $", '{groupId}'";
                else command.CommandText += $", null";

                command.CommandText += ");";
                command.Connection = _connectionUseTestDb;
                await command.PrepareAsync();
                var reader = await command.ExecuteReaderAsync();
                var newPermit = new Permit();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newPermit = map.Permit(reader);
                }
                await reader.CloseAsync();
                return newPermit;
            }
        }

        private async Task SeedPermitApiDllAdminUser()
        {
            if (_keeperApiDll is null || _userAdmin is null) throw new Exception("Could not create permit.");
            await SeedPermit(_keeperApiDll.Id, _userAdmin.Id, null);
        }

        private async Task SeedPermitDataDllAdminsGroup()
        {
            if (_keeperDataDll is null || _groupAdmins is null) throw new Exception("Could not create permit.");
            await SeedPermit(_keeperDataDll.Id, null, _groupAdmins.Id);
        }

        private async Task SeedPermitDomainDllManagerUser()
        {
            if (_keeperDomainDll is null || _userManager is null) throw new Exception("Could not create permit.");
            await SeedPermit(_keeperDomainDll.Id, _userManager.Id, null);
        }

        private async Task SeedPermitServiceDllManagersGroup()
        {
            if (_keeperServiceDll is null || _groupManagers is null) throw new Exception("Could not create permit.");
            await SeedPermit(_keeperServiceDll.Id, null, _groupManagers.Id);
        }

        private async Task SeedPermitTestDllBasicUser()
        {
            if (_keeperTestDll is null || _userBasic is null) throw new Exception("Could not create permit.");
            await SeedPermit(_keeperTestDll.Id, _userBasic.Id, null);
        }

        #endregion PERMITS
    }
}
