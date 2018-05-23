  
using System;
using System.Data;
using System.Data.SqlClient;

namespace DapperUnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _disposed;

        /// <summary>
        /// Gets current instance of the NhUnitOfWork.
        /// It gets the right instance that is related to current thread.
        /// </summary>
        public static UnitOfWork Current
        {
            get { return _current; }
            set { _current = value; }
        }
        [ThreadStatic]
        private static UnitOfWork _current;
        public UnitOfWork()
        {
            _connection = new SqlConnection();
            _connection.Open();
        }

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        public void BeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _connection.Close();
            }
        }

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        public void Rollback()
        {
            try
            {
                _transaction.Rollback();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _connection.Close();
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if(disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if(_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            dispose(false);
        }
    }
}
