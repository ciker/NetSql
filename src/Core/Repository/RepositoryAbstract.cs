using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Td.Fw.Data.Core.Entities;
using Td.Fw.Data.Core.Pagination;
using Td.Fw.Data.Core.SqlQueryable.Abstract;

namespace Td.Fw.Data.Core.Repository
{
    /// <summary>
    /// 仓储抽象类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract partial class RepositoryAbstract<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly IDbSet<TEntity> Db;
        protected readonly IDbContext DbContext;

        protected RepositoryAbstract(IDbContext dbContext)
        {
            DbContext = dbContext;
            Db = dbContext.Set<TEntity>();
        }

        public bool Exists(Expression<Func<TEntity, bool>> where, IDbTransaction transaction = null)
        {
            return Db.Find(where).Exists();
        }

        public virtual bool Add(TEntity entity, IDbTransaction transaction = null)
        {
            if (entity == null)
                return false;

            return Db.Insert(entity, transaction);
        }

        public virtual bool Add(List<TEntity> list, IDbTransaction transaction = null, int flushSize = 0)
        {
            return Db.BatchInsert(list, transaction, flushSize);
        }

        public virtual bool Delete(dynamic id, IDbTransaction transaction = null)
        {
            return Db.Delete(id, transaction);
        }

        public virtual bool Update(TEntity entity, IDbTransaction transaction = null)
        {
            return Db.Update(entity, transaction);
        }

        public virtual TEntity Get(dynamic id, IDbTransaction transaction = null)
        {
            return Db.Get(id, transaction);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> @where, IDbTransaction transaction = null)
        {
            return Db.Find(where).First();
        }

        public IList<TEntity> GetAll(IDbTransaction transaction = null)
        {
            return Db.Find().ToList();
        }

        public virtual IList<TEntity> Pagination(Paging paging = null, Expression<Func<TEntity, bool>> where = null, IDbTransaction transaction = null)
        {
            var query = Db.Find(where);

            return Pagination(paging, query, transaction);
        }

        protected IList<TEntity> Pagination(Paging paging, INetSqlQueryable<TEntity> query, IDbTransaction transaction = null)
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
    }
}
