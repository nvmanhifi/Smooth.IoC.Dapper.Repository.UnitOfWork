﻿using System;
using System.Data;

namespace Smooth.IoC.Dapper.Repository.UnitOfWork.Data
{
    public abstract class DbTransaction : IDbTransaction
    {
        private readonly IDbFactory _factory;
        protected bool Disposed;
        protected ISession Session;
        public IDbTransaction Transaction { get; set; }
        public IDbConnection Connection => Transaction.Connection;
        public IsolationLevel IsolationLevel => Transaction?.IsolationLevel ?? IsolationLevel.Unspecified;

        protected DbTransaction(IDbFactory factory)
        {
            _factory = factory;
        }

        [Obsolete("Use will commit on disposal")]
        public void Commit()
        {
            if (Connection?.State == ConnectionState.Open)
            {
                Transaction?.Commit();
            }
        }

        public void Rollback()
        {
            if (Connection?.State == ConnectionState.Open)
            {
                Transaction?.Rollback();
            }
        }

        ~DbTransaction()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {

            if (Disposed) return;
            Disposed = true;
            if (!disposing) return;
            DisposeTransaction();
            DisposeSessionIfSessionIsNotNull();
        }

        private void DisposeTransaction()
        {
            if (Transaction?.Connection == null) return;
            try
            {
                Commit();
                Transaction?.Dispose();
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                Transaction = null;
                _factory.Release(this);
            }
        }
        private void DisposeSessionIfSessionIsNotNull()
        {
            Session?.Dispose();
            Session = null;
        }
    }
}
