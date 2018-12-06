using System;
using System.Linq.Expressions;
using NetSql.Core.Entities;

namespace NetSql.Core.SqlQueryable.Abstract
{
    public interface INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4>
        : INetSqlQueryableBase<TEntity, INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4>,
            Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>>>
        where TEntity : Entity, new()
        where TEntity2 : Entity, new()
        where TEntity3 : Entity, new()
        where TEntity4 : Entity, new()
    {

        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> Select<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TResult>> selectExpression);

        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TKey>> expression);

        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TKey>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="TEntity5"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> LeftJoin<TEntity5>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression) where TEntity5 : Entity, new();

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="TEntity5"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> InnerJoin<TEntity5>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression) where TEntity5 : Entity, new();

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="TEntity5"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> RightJoin<TEntity5>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>> onExpression) where TEntity5 : Entity, new();
    }
}
