using Npgsql;
using TrappyKeepy.Data.Repositories;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Data
{
    /// <summary>
    /// Unit of work.
    /// Group operations into a single transaction so that all operations either
    /// pass or fail as one unit.
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Private repository objects.

        private IFiledataRepository? _filedataRepository;
        private IGroupRepository? _groupRepository;
        private IKeeperRepository? _keeperRepository;
        private IMembershipRepository? _membershipRepository;
        private IPermitRepository? _permitRepository;
        private IUserRepository? _userRepository;

        #endregion Private repository objects.

        // Private database connection and transaction.
        private string _connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        private NpgsqlConnection _connection;
        private NpgsqlTransaction? _transaction;

        private bool disposed;

        /// <summary>
        /// Constructor.
        /// Instantiate the Npgsql connection and open it.
        /// </summary>
        public UnitOfWork()
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }

        #region Transaction methods.

        /// <summary>
        /// Begin a new transaction.
        /// </summary>
        public void Begin()
        {
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// Commit the current transaction.
        /// </summary>
        public void Commit()
        {
            if (_transaction is not null) _transaction.Commit();
            if (_transaction is not null && _transaction.Connection is not null) _transaction.Connection.Close();
            if (_connection is not null) _connection.Close();
        }

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        public void Rollback()
        {
            if (_transaction is not null) _transaction.Rollback();
            if (_transaction is not null && _transaction.Connection is not null) _transaction.Connection.Close();
        }

        #endregion Transaction methods.

        #region Public repository objects.

        /// <summary>
        /// The filedatas repository.
        /// </summary>
        /// <value></value>
        public IFiledataRepository filedatas
        {
            get
            {
                if (_filedataRepository is null) _filedataRepository = new FiledataRepository(_connection);
                return _filedataRepository;
            }
        }

        /// <summary>
        /// The groups repository.
        /// </summary>
        /// <value></value>
        public IGroupRepository groups
        {
            get
            {
                if (_groupRepository is null) _groupRepository = new GroupRepository(_connection);
                return _groupRepository;
            }
        }

        /// <summary>
        /// The keeper repository.
        /// </summary>
        /// <value></value>
        public IKeeperRepository keepers
        {
            get
            {
                if (_keeperRepository is null) _keeperRepository = new KeeperRepository(_connection);
                return _keeperRepository;
            }
        }

        /// <summary>
        /// The memberships repository.
        /// </summary>
        /// <value></value>
        public IMembershipRepository memberships
        {
            get
            {
                if (_membershipRepository is null) _membershipRepository = new MembershipRepository(_connection);
                return _membershipRepository;
            }
        }

        /// <summary>
        /// The permits repository.
        /// </summary>
        /// <value></value>
        public IPermitRepository permits
        {
            get
            {
                if (_permitRepository is null) _permitRepository = new PermitRepository(_connection);
                return _permitRepository;
            }
        }

        /// <summary>
        /// The user repository.
        /// </summary>
        /// <value></value>
        public IUserRepository users
        {
            get
            {
                if (_userRepository is null) _userRepository = new UserRepository(_connection);
                return _userRepository;
            }
        }

        #endregion Public repository objects.

        #region Garbage collection.

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // If we are in a transaction, but the connection wasn't closed,
                // then something went wrong. The connection should be closed
                // immediately after the transaction commit.
                if (_transaction is not null && _transaction.Connection is not null)
                {
                    _transaction.Rollback();
                    _transaction.Connection.Close();
                }
                if (_connection is not null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
                disposed = true;
            }
        }

        #endregion Garbage collection.
    }
}
