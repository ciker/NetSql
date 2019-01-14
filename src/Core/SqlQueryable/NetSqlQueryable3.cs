using System;
using System.Linq.Expressions;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.Enums;
using NetSql.Abstractions.Pagination;
using NetSql.Abstractions.SqlQueryable;
using NetSql.Core.Internal;

namespace NetSql.Core.SqlQueryable
{
    internal class NetSqlQueryable<TEntity, TEntity2, TEntity3> : NetSqlQueryableAbstract<TEntity>, INetSqlQueryable<TEntity, TEntity2, TEntity3> where TEntity : IEntity, new() where TEntity2 : IEntity, new() where TEntity3 : IEntity, new()
    {
        public NetSqlQueryable(IDbSet<TEntity> dbSet, Expression<Func<TEntity, TEntity2, TEntity3, bool>> onExpression, JoinType joinType = JoinType.Left, JoinCollection joinCollection = null, QueryableBody queryableBody = null)
            : base(dbSet, typeof(Func<,,,>).MakeGenericType(typeof(TEntity), typeof(TEntity2), typeof(TEntity3), typeof(bool)), joinCollection, queryableBody)
        {
            Check.NotNull(onExpression, nameof(onExpression), "请输入连接条件");

            SetJoin(new JoinDescriptor
            {
                Type = joinType,
                Alias = "T3",
                EntityDescriptor = dbSet.DbContext.Options.EntityDescriptorCollection.Get<TEntity3>(),
                On = onExpression
            });
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> Where(Expression<Func<TEntity, TEntity2, TEntity3, bool>> whereExpression)
        {
            SetWhere(whereExpression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> WhereIf(bool isAdd, Expression<Func<TEntity, TEntity2, TEntity3, bool>> expression)
        {
            if (isAdd)
                Where(expression);

            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> Limit(int skip, int take)
        {
            SetLimit(skip, take);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> Select<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TResult>> selectExpression)
        {
            SetSelect(selectExpression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TKey>> expression)
        {
            SetOrderBy(expression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> OrderBy(string colName)
        {
            return Order(new Sort(colName));
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> Order(Sort sort)
        {
            SetOrderBy(sort);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TKey>> expression)
        {
            SetOrderBy(expression, SortType.Desc);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> LeftJoin<TEntity4>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> onExpression) where TEntity4 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4>(Db, onExpression, JoinType.Left, JoinCollection, QueryableBody);
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> InnerJoin<TEntity4>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> onExpression) where TEntity4 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4>(Db, onExpression, JoinType.Inner, JoinCollection, QueryableBody);
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> RightJoin<TEntity4>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> onExpression) where TEntity4 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4>(Db, onExpression, JoinType.Right, JoinCollection, QueryableBody);
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> OrderByDescending(string colName)
        {
            return Order(new Sort(colName, SortType.Desc));
        }
    }
}
