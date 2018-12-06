using System;
using System.Linq.Expressions;
using Td.Fw.Data.Core.Entities;
using Td.Fw.Data.Core.Entities.@internal;
using Td.Fw.Data.Core.Enums;
using Td.Fw.Data.Core.Internal;
using Td.Fw.Data.Core.Pagination;
using Td.Fw.Data.Core.SqlAdapter;
using Td.Fw.Data.Core.SqlQueryable.Abstract;

namespace Td.Fw.Data.Core.SqlQueryable.Impl
{
    internal class NetSqlQueryable<TEntity, TEntity2> : NetSqlQueryableAbstract<TEntity>, INetSqlQueryable<TEntity, TEntity2> where TEntity : Entity, new() where TEntity2 : Entity, new()
    {
        public NetSqlQueryable(IDbSet<TEntity> db, ISqlAdapter sqlAdapter, Expression<Func<TEntity, TEntity2, bool>> onExpression, JoinType joinType = JoinType.Left)
            : base(db, sqlAdapter, typeof(Func<,,>).MakeGenericType(typeof(TEntity), typeof(TEntity2), typeof(bool)))
        {
            Check.NotNull(onExpression, nameof(onExpression), "请输入连接条件");

            SetJoin(new JoinDescriptor
            {
                Type = joinType,
                Alias = "T2",
                EntityDescriptor = EntityCollection.TryGet<TEntity2>(),
                On = onExpression
            });
        }

        public INetSqlQueryable<TEntity, TEntity2> Where(Expression<Func<TEntity, TEntity2, bool>> whereExpression)
        {
            SetWhere(whereExpression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2> WhereIf(bool isAdd, Expression<Func<TEntity, TEntity2, bool>> expression)
        {
            if (isAdd)
                Where(expression);

            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2> Limit(int skip, int take)
        {
            SetLimit(skip, take);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2> Select<TResult>(Expression<Func<TEntity, TEntity2, TResult>> selectExpression)
        {
            SetSelect(selectExpression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> LeftJoin<TEntity3>(Expression<Func<TEntity, TEntity2, TEntity3, bool>> onExpression) where TEntity3 : Entity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3>(Db, SqlAdapter, onExpression, JoinType.Left, JoinCollection, QueryableBody);
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> InnerJoin<TEntity3>(Expression<Func<TEntity, TEntity2, TEntity3, bool>> onExpression) where TEntity3 : Entity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3>(Db, SqlAdapter, onExpression, JoinType.Inner, JoinCollection, QueryableBody);
        }

        public INetSqlQueryable<TEntity, TEntity2, TEntity3> RightJoin<TEntity3>(Expression<Func<TEntity, TEntity2, TEntity3, bool>> onExpression) where TEntity3 : Entity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2, TEntity3>(Db, SqlAdapter, onExpression, JoinType.Right, JoinCollection, QueryableBody);
        }

        public INetSqlQueryable<TEntity, TEntity2> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TKey>> expression)
        {
            SetOrderBy(expression);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2> OrderBy(string colName)
        {
            return Order(new Sort(colName));
        }

        public INetSqlQueryable<TEntity, TEntity2> Order(Sort sort)
        {
            SetOrderBy(sort);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TKey>> expression)
        {
            SetOrderBy(expression, SortType.Desc);
            return this;
        }

        public INetSqlQueryable<TEntity, TEntity2> OrderByDescending(string colName)
        {
            return Order(new Sort(colName, SortType.Desc));
        }
    }
}
