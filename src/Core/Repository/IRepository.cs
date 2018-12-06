using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using NetSql.Core.Entities;
using NetSql.Core.Pagination;

namespace NetSql.Core.Repository
{
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial interface IRepository<TEntity> where TEntity : Entity, new()
    {
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<TEntity, bool>> where, IDbTransaction transaction = null);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        bool Add(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <param name="transaction">事务</param>
        /// <param name="flushSize">单次执行sql语句大小,单位KB</param>
        /// <returns></returns>
        bool Add(List<TEntity> list, IDbTransaction transaction = null, int flushSize = 2048);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Delete(dynamic id, IDbTransaction transaction = null);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        bool Update(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        TEntity Get(dynamic id, IDbTransaction transaction = null);

        /// <summary>
        /// 根据表达式查询单条记录
        /// </summary>
        /// <param name="where"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> where, IDbTransaction transaction = null);

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        IList<TEntity> GetAll(IDbTransaction transaction = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="where">过滤条件</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        IList<TEntity> Pagination(Paging paging = null, Expression<Func<TEntity, bool>> where = null, IDbTransaction transaction = null);
    }
}
