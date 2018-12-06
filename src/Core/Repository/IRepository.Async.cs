﻿#if !NET40

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Core.Entities;
using NetSql.Core.Pagination;

namespace NetSql.Core.Repository
{
    public partial interface IRepository<TEntity> where TEntity : Entity, new()
    {
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where, IDbTransaction transaction = null);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <param name="transaction">事务</param>
        /// <param name="flushSize">单次执行sql语句大小,单位KB</param>
        /// <returns></returns>
        Task<bool> AddAsync(List<TEntity> list, IDbTransaction transaction = null, int flushSize = 2048);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(dynamic id, IDbTransaction transaction = null);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null);

        /// <summary>
        /// 根据表达式查询单条记录
        /// </summary>
        /// <param name="where"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, IDbTransaction transaction = null);

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<IList<TEntity>> GetAllAsync(IDbTransaction transaction = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="where">过滤条件</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<IList<TEntity>> PaginationAsync(Paging paging = null, Expression<Func<TEntity, bool>> where = null, IDbTransaction transaction = null);
    }
}

#endif