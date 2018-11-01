using System;
using System.Linq.Expressions;
using NetSql.Entities;
using NetSql.Enums;
using NetSql.Internal;
using NetSql.Pagination;
using NetSql.SqlAdapter;
using NetSql.SqlQueryable.Abstract;

namespace NetSql.SqlQueryable.Impl
{
    internal class NetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>
        : NetSqlQueryableAbstract<TEntity>, INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>
        where TEntity : Entity, new()
        where TEntity2 : Entity, new()
        where TEntity3 : Entity, new()
        where TEntity4 : Entity, new()
        where TEntity5 : Entity, new()
    {
        public NetSqlQueryable(IDbSet<TEntity> db, ISqlAdapter sqlAdapter, Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression, JoinType joinType, JoinCollection joinCollection = null, QueryableBody queryableBody = null)
            : base(db, sqlAdapter, typeof(Func<,,,,,>).MakeGenericType(typeof(TEntity), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4), typeof(TEntity5), typeof(bool)), joinCollection, queryableBody)
        {
            Check.NotNull(onExpression, nameof(onExpression), "请输入连接条件");

            SetJoin(new JoinDescriptor
            {
                Type = joinType,
                Alias = "T5",
                EntityDescriptor = EntityCollection.TryGet<TEntity5>(),
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
