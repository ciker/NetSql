using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.Enums;
using NetSql.Abstractions.Pagination;
using NetSql.Abstractions.SqlQueryable;
using NetSql.Core.SqlQueryable.Internal;

namespace NetSql.Core.SqlQueryable
{
    internal class NetSqlQueryable<TEntity> : NetSqlQueryableAbstract, INetSqlQueryable<TEntity> where TEntity : IEntity, new()
    {
        private readonly EntitySql _sql;

        public NetSqlQueryable(IDbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> whereExpression) : base(dbSet, new QueryBody(dbSet.DbContext.Options.SqlAdapter))
        {
            _sql = dbSet.EntityDescriptor.Sql;

            QueryBody.JoinDescriptors.Add(new QueryJoinDescriptor
            {
                Type = JoinType.UnKnown,
                Alias = "T1",
                EntityDescriptor = dbSet.DbContext.Options.EntityDescriptorCollection.Get<TEntity>(),
            });
            QueryBody.WhereDelegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), typeof(bool));

            Where(whereExpression);
        }

        #region ==Sort==

        public INetSqlQueryable<TEntity> OrderBy(string colName)
        {
            return Order(new Sort(colName));
        }

        public INetSqlQueryable<TEntity> OrderByDescending(string colName)
        {
            return Order(new Sort(colName, SortType.Desc));
        }

        public INetSqlQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            QueryBody.SetOrderBy(expression);
            return this;
        }

        public INetSqlQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            QueryBody.SetOrderBy(expression, SortType.Desc);
            return this;
        }

        public INetSqlQueryable<TEntity> Order(Sort sort)
        {
            QueryBody.SetOrderBy(sort);
            return this;
        }

        #endregion

        #region ==Where==

        public INetSqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            QueryBody.SetWhere(expression);
            return this;
        }

        public INetSqlQueryable<TEntity> WhereIf(bool ifCondition, Expression<Func<TEntity, bool>> expression)
        {
            if (ifCondition)
                Where(expression);

            return this;
        }

        #endregion

        #region ==Limit==

        public INetSqlQueryable<TEntity> Limit(int skip, int take)
        {
            QueryBody.SetLimit(skip, take);
            return this;
        }

        #endregion

        #region ==Select==

        public INetSqlQueryable<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            QueryBody.SetSelect(expression);
            return this;
        }

        #endregion

        #region ==Join==

        public INetSqlQueryable<TEntity, TEntity2> LeftJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2>(Db, QueryBody, onExpression);
        }

        public INetSqlQueryable<TEntity, TEntity2> InnerJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2>(Db, QueryBody, onExpression, JoinType.Inner);
        }

        public INetSqlQueryable<TEntity, TEntity2> RightJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2>(Db, QueryBody, onExpression, JoinType.Right);
        }

        #endregion

        #region ==First==

        public TEntity First()
        {
            return First<TEntity>();
        }

        public Task<TEntity> FirstAsync()
        {
            return FirstAsync<TEntity>();
        }

        #endregion

        #region ==Delete==

        public bool Delete()
        {
            DeleteWithAffectedNum();
            return true;
        }

        public Task<bool> DeleteAsync()
        {
            DeleteWithAffectedNumAsync();
            return Task.FromResult(true);
        }

        public int DeleteWithAffectedNum()
        {
            var sql = QueryBuilder.DeleteSqlBuild(Db.EntityDescriptor.TableName, out QueryParameters parameters);
            return Db.Execute(sql, parameters.Parse());
        }

        public Task<int> DeleteWithAffectedNumAsync()
        {
            var sql = QueryBuilder.DeleteSqlBuild(Db.EntityDescriptor.TableName, out QueryParameters parameters);
            return Db.ExecuteAsync(sql, parameters.Parse());
        }

        #endregion

        #region ==Update==

        public bool Update(Expression<Func<TEntity, TEntity>> expression)
        {
            UpdateWithAffectedNum(expression);
            return true;
        }

        public Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> expression)
        {
            UpdateWithAffectedNumAsync(expression);
            return Task.FromResult(true);
        }

        public int UpdateWithAffectedNum(Expression<Func<TEntity, TEntity>> expression)
        {
            QueryBody.Update = expression;
            var sql = QueryBuilder.UpdateSqlBuild(Db.EntityDescriptor.TableName, out QueryParameters parameters);
            return Db.Execute(sql, parameters.Parse());
        }

        public Task<int> UpdateWithAffectedNumAsync(Expression<Func<TEntity, TEntity>> expression)
        {
            QueryBody.Update = expression;
            var sql = QueryBuilder.UpdateSqlBuild(Db.EntityDescriptor.TableName, out QueryParameters parameters);
            return Db.ExecuteAsync(sql, parameters.Parse());
        }

        #endregion

        #region ==Max==

        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return base.Max<TResult>(expression);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return base.MaxAsync<TResult>(expression);
        }

        #endregion

        #region ==Min==


        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return base.Min<TResult>(expression);
        }

        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return base.MinAsync<TResult>(expression);
        }

        #endregion

        #region ==ToList==

        public IList<TEntity> ToList()
        {
            return ToList<TEntity>();
        }

        public Task<IList<TEntity>> ToListAsync()
        {
            return ToListAsync<TEntity>();
        }

        #endregion

        #region ==Pagination==

        public IList<TEntity> Pagination(Paging paging = null)
        {
            return Pagination<TEntity>(paging);
        }

        public Task<IList<TEntity>> PaginationAsync(Paging paging = null)
        {
            return PaginationAsync<TEntity>(paging);
        }

        #endregion
    }
}
