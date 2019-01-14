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
    internal class NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>
        : NetSqlQueryableAbstract<TEntity>, INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>
        where TEntity : IEntity, new()
        where TEntity2 : IEntity, new()
        where TEntity3 : IEntity, new()
        where TEntity4 : IEntity, new()
        where TEntity5 : IEntity, new()
    {
        public NetSqlQueryable(IDbSet<TEntity> dbSet, Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression, JoinType joinType, JoinCollection joinCollection = null, QueryableBody queryableBody = null)
            : base(dbSet, typeof(Func<,,,,,>).MakeGenericType(typeof(TEntity), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(bool)), joinCollection, queryableBody)
        {
            Check.NotNull(onExpression, nameof(onExpression), "请输入连接条件");

            SetJoin(new JoinDescriptor
            {
                Type = joinType,
                Alias = "T5",
                EntityDescriptor = dbSet.DbContext.Options.EntityDescriptorCollection.Get<TEntity5>(),
                On = onExpression
            });
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> Where(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> expression)
        {
            SetWhere(expression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> WhereIf(bool isAdd, Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> expression)
        {
            if (isAdd)
                Where(expression);

            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> OrderBy(string colName)
        {
            return Order(new Sort(colName));
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> OrderByDescending(string colName)
        {
            return Order(new Sort(colName, SortType.Desc));
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> Order(Sort sort)
        {
            SetOrderBy(sort);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> Limit(int skip, int take)
        {
            SetLimit(skip, take);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> Select<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TResult>> selectExpression)
        {
            SetSelect(selectExpression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TKey>> expression)
        {
            SetOrderBy(expression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TKey>> expression)
        {
            SetOrderBy(expression, SortType.Desc);
            return this;
        }
    }
}
