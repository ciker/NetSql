using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.Pagination;
using NetSql.Abstractions.SqlQueryable;

namespace NetSql.Core
{
    /// <summary>
    /// 仓储抽象类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class RepositoryAbstract<TEntity> : IRepository<TEntity> where TEntity : IEntity, new()
    {
        /// <summary>
        /// 数据集
        /// </summary>
        protected readonly IDbSet<TEntity> Db;

        protected RepositoryAbstract(IDbContext context)
        {
            Db = context.Set<TEntity>();
        }

        #region ==Exists==

        public bool Exists(Expression<Func<TEntity, bool>> where)
        {
            return Db.Find(where).Exists();
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where)
        {
            return Db.Find(where).ExistsAsync();
        }

        #endregion

        #region ==Add==

        public virtual bool Add(TEntity entity)
        {
            if (entity == null)
                return false;

            return Db.Insert(entity);
        }

        public virtual Task<bool> AddAsync(TEntity entity)
        {
            if (entity == null)
                return Task.FromResult(false);

            return Db.InsertAsync(entity);
        }

        public virtual bool Add(List<TEntity> list, int flushSize = 0)
        {
            return Db.BatchInsert(list, flushSize);
        }

        public virtual Task<bool> AddAsync(List<TEntity> list, int flushSize = 0)
        {
            return Db.BatchInsertAsync(list, flushSize);
        }

        #endregion

        #region ==Delete==

        public virtual bool Remove(dynamic id)
        {
            return Db.Delete(id);
        }

        public virtual Task<bool> RemoveAsync(dynamic id)
        {
            return Db.DeleteAsync(id);
        }

        #endregion

        #region ==Update==

        public virtual bool Update(TEntity entity)
        {
            return Db.Update(entity);
        }

        public virtual Task<bool> UpdateAsync(TEntity entity)
        {
            return Db.UpdateAsync(entity);
        }

        #endregion

        #region ==Get==

        public virtual TEntity Get(dynamic id)
        {
            return Db.Get(id);
        }

        public virtual Task<TEntity> GetAsync(dynamic id)
        {
            return Db.GetAsync(id);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> @where)
        {
            return Db.Find(where).First();
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> @where)
        {
            return Db.Find(where).FirstAsync();
        }

        #endregion

        #region ==GetAll==

        public IList<TEntity> GetAll()
        {
            return Db.Find().ToList();
        }

        public Task<IList<TEntity>> GetAllAsync()
        {
            return Db.Find().ToListAsync();
        }

        #endregion

        #region ==Pagination==

        public virtual IList<TEntity> Pagination(Paging paging = null, Expression<Func<TEntity, bool>> where = null)
        {
            var query = Db.Find(where);

            return Pagination(paging, query);
        }

        public virtual Task<IList<TEntity>> PaginationAsync(Paging paging = null, Expression<Func<TEntity, bool>> where = null)
        {
            var query = Db.Find(where);

            return PaginationAsync(paging, query);
        }

        protected IList<TEntity> Pagination(Paging paging, INetSqlQueryable<TEntity> query)
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

            return query.Pagination(paging);
        }

        protected Task<IList<TEntity>> PaginationAsync(Paging paging, INetSqlQueryable<TEntity> query)
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

        #endregion
    }
}
