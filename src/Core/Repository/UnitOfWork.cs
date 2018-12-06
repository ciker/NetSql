using System.Data;

namespace NetSql.Core.Repository
{
    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : IDbContext
    {
        private readonly IDbContext _context;
        private IDbTransaction _transaction;

        public UnitOfWork(TDbContext context)
        {
            _context = context;
        }

        public IDbTransaction BeginTransaction()
        {
            _transaction = _context.BeginTransaction();
            return _transaction;
        }

        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Connection.Close();
            }
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Connection.Close();
            }
        }
    }
}
