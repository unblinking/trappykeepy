using Npgsql;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Data.Repositories;

namespace TrappyKeepy.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private UserRepository? userRepository;
        private KeeperRepository? keeperRepository;
        private FilebyteaRepository? filebyteaRepository;
        private string connectionString;
        private NpgsqlConnection connection;
        private NpgsqlTransaction? transaction;
        private bool disposed;


        public UnitOfWork(string connectionString, bool useTransaction)
        {
            this.connectionString = connectionString;
            this.connection = new NpgsqlConnection(connectionString);
            this.connection.Open();
            if (useTransaction)
            {
                this.transaction = connection.BeginTransaction();
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository is null)
                {
                    userRepository = new UserRepository(connection);
                }
                return userRepository;
            }
        }

        public KeeperRepository KeeperRepository
        {
            get
            {
                if (this.keeperRepository is null)
                {
                    keeperRepository = new KeeperRepository(connection);
                }
                return keeperRepository;
            }
        }

        public FilebyteaRepository FilebyteaRepository
        {
            get
            {
                if (this.filebyteaRepository is null)
                {
                    filebyteaRepository = new FilebyteaRepository(connection);
                }
                return filebyteaRepository;
            }
        }

        public void Commit()
        {
            if (transaction is not null)
            {
                transaction.Commit();
            }
            if (transaction is not null && transaction.Connection is not null)
            {
                transaction.Connection.Close();
            }
            if (connection is not null)
            {
                connection.Close();
            }
        }

        public void Rollback()
        {
            if (transaction is not null)
            {
                transaction.Rollback();
            }
            if (transaction is not null && transaction.Connection is not null)
            {
                transaction.Connection.Close();
            }
        }

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
    }
}