using System;
using System.Linq.Expressions;
using Td.Fw.Data.Core.Entities;

namespace Td.Fw.Data.Core.SqlQueryable.Abstract
{

    public interface INetSqlQueryable<TEntity, TEntity2> : INetSqlQueryableBase<TEntity, INetSqlQueryable<TEntity, TEntity2>, Expression<Func<TEntity, TEntity2, bool>>> where TEntity : Entity, new() where TEntity2 : Entity, new()
    {
        /// <summary>
        /// 升序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression">列</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TKey>> expression);

        /// <summary>
        /// 降序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TKey>> expression);

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selectExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> Select<TResult>(Expression<Func<TEntity, TEntity2, TResult>> selectExpression);

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="TEntity3"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3> LeftJoin<TEntity3>(Expression<Func<TEntity, TEntity2, TEntity3, bool>> onExpression) where TEntity3 : Entity, new();

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="TEntity3"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3> InnerJoin<TEntity3>(Expression<Func<TEntity, TEntity2, TEntity3, bool>> onExpression) where TEntity3 : Entity, new();

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="TEntity3"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2, TEntity3> RightJoin<TEntity3>(Expression<Func<TEntity, TEntity2, TEntity3, bool>> onExpression) where TEntity3 : Entity, new();
    }
}