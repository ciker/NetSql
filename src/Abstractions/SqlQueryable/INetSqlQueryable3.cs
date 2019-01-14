using System;
using System.Linq.Expressions;
using NetSql.Abstractions.Entities;

namespace NetSql.Abstractions.SqlQueryable
{
    public interface INetSqlQueryable<TEntity, TEntity2, TEntity3> : INetSqlQueryableBase<TEntity, INetSqlQueryable<TEntity, TEntity2, TEntity3>, Expression<Func<TEntity, TEntity2, TEntity3, bool>>> where TEntity : IEntity, new() where TEntity2 : IEntity, new() where TEntity3 : IEntity, new()
    {
        INetSqlQueryable<TEntity, TEntity2, TEntity3> Select<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TResult>> selectExpression);

        INetSqlQueryable<TEntity, TEntity2, TEntity3> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TKey>> expression);

        INetSqlQueryable<TEntity, TEntity2, TEntity3> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TKey>> expression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="TEntity4"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> LeftJoin<TEntity4>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> onExpression) where TEntity4 : IEntity, new();

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="TEntity4"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> InnerJoin<TEntity4>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> onExpression) where TEntity4 : IEntity, new();

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="TEntity4"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> RightJoin<TEntity4>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, bool>> onExpression) where TEntity4 : IEntity, new();
    }
}
