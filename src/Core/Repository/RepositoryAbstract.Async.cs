#if !NET40

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Core.Entities;
using NetSql.Core.Pagination;
using NetSql.Core.SqlQueryable.Abstract;

namespace NetSql.Core.Repository
{
    public abstract partial class RepositoryAbstract<TEntity> where TEntity : Entity, new()
    {
        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where, IDbTransaction transaction = null)
        {
            return Db.Find(where).ExistsAsync();
        }

        public virtual Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null)
        {
            if (entity == null)
                return Task.FromResult(false);

            return Db.InsertAsync(entity, transaction);
        }

        public virtual Task<bool> AddAsync(List<TEntity> list, IDbTransaction transaction = null, int flushSize = 0)
        {
            return Db.BatchInsertAsync(list, transaction, flushSize);
        }

        public virtual Task<bool> DeleteAsync(dynamic id, IDbTransaction transaction = null)
        {
            return Db.DeleteAsync(id, transaction);
        }

        public virtual Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return Db.UpdateAsync(entity, transaction);
        }

        public virtual Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null)
        {
            return Db.GetAsync(id, transaction);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> @where, IDbTransaction transaction = null)
        {
            return Db.Find(where).FirstAsync();
        }

        public Task<IList<TEntity>> GetAllAsync(IDbTransaction transaction = null)
        {
            return Db.Find().ToListAsync();
        }

        public virtual Task<IList<TEntity>> PaginationAsync(Paging paging = null, Expression<Func<TEntity, bool>> where = null, IDbTransaction transaction = null)
        {
            var query = Db.Find(where);

            return PaginationAsync(paging, query, transaction);
        }

        protected Task<IList<TEntity>> PaginationAsync(Paging paging, INetSqlQueryable<TEntity> query, IDbTransaction transaction = null)
        {
            if (paging == null)
                paging = new Paging();

            if (query == null)
                query = Db.Find();

            //排序
            foreach (var sort in paging.OrderBy)
            {
                query.Order(sort);
            }

            return query.PaginationAsync(paging);
        }
    }
}

#endif