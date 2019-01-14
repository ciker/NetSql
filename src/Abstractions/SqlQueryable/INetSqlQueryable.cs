using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Abstractions.Entities;

namespace NetSql.Abstractions.SqlQueryable
{
    /// <summary>
    /// Sql构造器
    /// </summary>
    public interface INetSqlQueryable<TEntity> : INetSqlQueryableBase<TEntity, INetSqlQueryable<TEntity>, Expression<Func<TEntity, bool>>> where TEntity : IEntity, new()
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
        bool Delete();

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        bool Update(Expression<Func<TEntity, TEntity>> expression);

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
        /// <para>数据不存在也是返回true</para>
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAsync();

        /// <summary>
        /// 更新
        /// <para>数据不存在也是返回true</para>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> expression);

        /// <summary>
        /// 删除数据返回影响条数
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteWithAffectedNumAsync();

        /// <summary>
        /// 更新数据返回影响条数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> UpdateWithAffectedNumAsync(Expression<Func<TEntity, TEntity>> expression);
    }
}
