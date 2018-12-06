using System;
using System.Data;
using System.Linq.Expressions;
using Td.Fw.Data.Core.Entities;

namespace Td.Fw.Data.Core.SqlQueryable.Abstract
{
    /// <summary>
    /// Sql构造器
    /// </summary>
    public partial interface INetSqlQueryable<TEntity> : INetSqlQueryableBase<TEntity, INetSqlQueryable<TEntity>, Expression<Func<TEntity, bool>>> where TEntity : Entity, new()
    {
        /// <summary>
        /// 升序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expression);

        /// <summary>
        /// 降序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> expression);

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <returns></returns>
        TEntity First();

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        bool Delete(IDbTransaction transaction = null);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Update(Expression<Func<TEntity, TEntity>> expression, IDbTransaction transaction = null);
    }
}
