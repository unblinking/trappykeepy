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

        private IFiledataRepository? filedataRepository;
        private IGroupRepository? groupRepository;
        private IKeeperRepository? keeperRepository;
        private IMembershipRepository? membershipRepository;
        private IUserRepository? userRepository;

        #endregion Private repository objects.

        // Private database connection and transaction.
        private string connectionString = $"{Environment.GetEnvironmentVariable("TKDB_CONN_STRING")}";
        private NpgsqlConnection connection;
        private NpgsqlTransaction? transaction;

        private bool disposed;

        /// <summary>
        /// Constructor.
        /// Instantiate the Npgsql connection and open it.
        /// </summary>
        public UnitOfWork()
        {
            this.connection = new NpgsqlConnection(this.connectionString);
            this.connection.Open();
        }

        #region Transaction methods.

        /// <summary>
        /// Begin a new transaction.
        /// </summary>
        public void Begin()
        {
            this.transaction = connection.BeginTransaction();
        }

        /// <summary>
        /// Commit the current transaction.
        /// </summary>
        public void Commit()
        {
            if (transaction is not null) transaction.Commit();
            if (transaction is not null && transaction.Connection is not null) transaction.Connection.Close();
            if (connection is not null) connection.Close();
        }

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        public void Rollback()
        {
            if (transaction is not null) transaction.Rollback();
            if (transaction is not null && transaction.Connection is not null) transaction.Connection.Close();
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
                if (this.filedataRepository is null) filedataRepository = new FiledataRepository(connection);
                return filedataRepository;
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
                if (this.groupRepository is null) groupRepository = new GroupRepository(connection);
                return groupRepository;
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
                if (this.keeperRepository is null) keeperRepository = new KeeperRepository(connection);
                return keeperRepository;
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
                if (this.membershipRepository is null) membershipRepository = new MembershipRepository(connection);
                return membershipRepository;
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
                if (this.userRepository is null) userRepository = new UserRepository(connection);
                return userRepository;
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
                if (transaction is not null && transaction.Connection is not null)
                {
                    transaction.Rollback();
                    transaction.Connection.Close();
                }
                if (connection is not null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                disposed = true;
            }
        }

        #endregion Garbage collection.
    }
}
