#if !NET40

using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Core.Entities;

namespace NetSql.Core.SqlQueryable.Abstract
{
    public partial interface INetSqlQueryable<TEntity> : INetSqlQueryableBase<TEntity, INetSqlQueryable<TEntity>, Expression<Func<TEntity, bool>>> where TEntity : Entity, new()
    {

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <returns></returns>
        Task<bool> ExistsAsync();

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <returns></returns>
        Task<TEntity> FirstAsync();

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAsync(IDbTransaction transaction = null);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> expression, IDbTransaction transaction = null);
    }
}

#endif