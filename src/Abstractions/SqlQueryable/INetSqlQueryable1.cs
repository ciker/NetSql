using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.Pagination;

namespace NetSql.Abstractions.SqlQueryable
{
    /// <summary>
    /// Sql构造器
    /// </summary>
    public interface INetSqlQueryable<TEntity> : INetSqlQueryable where TEntity : IEntity, new()
    {
        #region ==Sort==

        /// <summary>
        /// 升序
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> OrderBy(string colName);

        /// <summary>
        /// 降序
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> OrderByDescending(string colName);

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
        /// 排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Order(Sort sort);

        #endregion

        #region ==Where==

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="expression">过滤条件</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="ifCondition">添加条件</param>
        /// <param name="expression">条件</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> WhereIf(bool ifCondition, Expression<Func<TEntity, bool>> expression);

        #endregion

        #region ==Limit==

        /// <summary>
        /// 限制
        /// </summary>
        /// <param name="skip">跳过前几条数据</param>
        /// <param name="take">取前几条数据</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Limit(int skip, int take);

        #endregion

        #region ==Select==

        /// <summary>
        /// 查询指定列
        /// </summary>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> expression);

        #endregion

        #region ==Join==

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="TEntity2"></typeparam>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> LeftJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new();

        /// <summary>
        /// 内连接
        /// </summary>
        /// <typeparam name="TEntity2"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> InnerJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new();

        /// <summary>
        /// 右连接
        /// </summary>
        /// <typeparam name="TEntity2"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> RightJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new();

        #endregion

        #region ==First==

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <returns></returns>
        TEntity First();

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <returns></returns>
        Task<TEntity> FirstAsync();

        #endregion

        #region ==Delete==

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        bool Delete();

        /// <summary>
        /// 删除
        /// <para>数据不存在也是返回true</para>
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAsync();

        /// <summary>
        /// 删除数据返回影响条数
        /// </summary>
        /// <returns></returns>
        int DeleteWithAffectedNum();

        /// <summary>
        /// 删除数据返回影响条数
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteWithAffectedNumAsync();

        #endregion

        #region ==Update==

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        bool Update(Expression<Func<TEntity, TEntity>> expression);

        /// <summary>
        /// 更新
        /// <para>数据不存在也是返回true</para>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> expression);

        /// <summary>
        /// 更新数据返回影响条数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        int UpdateWithAffectedNum(Expression<Func<TEntity, TEntity>> expression);

        /// <summary>
        /// 更新数据返回影响条数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> UpdateWithAffectedNumAsync(Expression<Func<TEntity, TEntity>> expression);

        #endregion

        #region ==Max==

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> expression);

        #endregion

        #region ==Min==

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<TEntity, TResult>> expression);

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> expression);

        #endregion

        #region ==ToList==

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <returns></returns>
        IList<TEntity> ToList();

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <returns></returns>
        Task<IList<TEntity>> ToListAsync();

        #endregion

        #region ==Pagination==

        /// <summary>
        /// 分页查询实体列表
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        IList<TEntity> Pagination(Paging paging = null);

        /// <summary>
        /// 分页查询实体列表
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<IList<TEntity>> PaginationAsync(Paging paging = null);

        #endregion
    }
}
