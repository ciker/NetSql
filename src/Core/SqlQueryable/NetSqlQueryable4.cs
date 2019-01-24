using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.Enums;
using NetSql.Abstractions.Pagination;
using NetSql.Abstractions.SqlQueryable;
using NetSql.Core.Internal;
using NetSql.Core.SqlQueryable.Internal;

namespace NetSql.Core.SqlQueryable
{
    internal class NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4>
        : NetSqlQueryableAbstract, INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4>
        where TEntity : IEntity, new()
        where TEntity2 : IEntity, new()
        where TEntity3 : IEntity, new()
        where TEntity4 : IEntity, new()
    {
        public NetSqlQueryable(IDbSet dbSet, QueryBody queryBody, Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> onExpression, JoinType joinType = JoinType.Left)
            : base(dbSet, queryBody)
        {
            Check.NotNull(onExpression, nameof(onExpression), "请输入连接条件");

            QueryBody.JoinDescriptors.Add(new QueryJoinDescriptor
            {
                Type = joinType,
                Alias = "T4",
                EntityDescriptor = dbSet.DbContext.Options.EntityDescriptorCollection.Get<TEntity4>(),
                On = onExpression
            });

            QueryBody.WhereDelegateType = typeof(Func<,,,,>).MakeGenericType(typeof(TEntity), typeof(TEntity2), typeof(TEntity3),
                typeof(TEntity4), typeof(bool));
        }
        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> OrderBy(string colName)
        {
            return Order(new Sort(colName));
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> OrderByDescending(string colName)
        {
            return Order(new Sort(colName, SortType.Desc));
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TKey>> expression)
        {
            QueryBody.SetOrderBy(expression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TKey>> expression)
        {
            QueryBody.SetOrderBy(expression, SortType.Desc);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> Order(Sort sort)
        {
            QueryBody.SetOrderBy(sort);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> Where(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> expression)
        {
            QueryBody.SetWhere(expression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> WhereIf(bool ifCondition, Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> expression)
        {
            if (ifCondition)
                Where(expression);

            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> Limit(int skip, int take)
        {
            QueryBody.SetLimit(skip, take);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> Select<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TResult>> selectExpression)
        {
            QueryBody.SetSelect(selectExpression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> LeftJoin<TEntity5>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression) where TEntity5 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>(Db, QueryBody, onExpression);
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> InnerJoin<TEntity5>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression) where TEntity5 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>(Db, QueryBody, onExpression, JoinType.Inner);
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> RightJoin<TEntity5>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression) where TEntity5 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>(Db, QueryBody, onExpression, JoinType.Right);
        }

        public TResult Max<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TResult>> expression)
        {
            return base.Max<TResult>(expression);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TResult>> expression)
        {
            return base.MaxAsync<TResult>(expression);
        }

        public TResult Min<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TResult>> expression)
        {
            return base.Min<TResult>(expression);
        }

        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TResult>> expression)
        {
            return base.MinAsync<TResult>(expression);
        }

        public IList<TEntity> ToList()
        {
            return ToList<TEntity>();
        }

        public Task<IList<TEntity>> ToListAsync()
        {
            return ToListAsync<TEntity>();
        }

        public IList<TEntity> Pagination(Paging paging = null)
        {
            return Pagination<TEntity>(paging);
        }

        public Task<IList<TEntity>> PaginationAsync(Paging paging = null)
        {
            return PaginationAsync<TEntity>(paging);
        }
    }
}
