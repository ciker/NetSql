﻿using System;
using NetSql.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.SqlQueryable.Abstract;

namespace NetSql
{
    /// <summary>
    /// 实体数据集
    /// </summary>
    public interface IDbSet
    {

    }

    /// <summary>
    /// 实体数据集
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDbSet<TEntity> : IDbSet where TEntity : Entity, new()
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task<bool> InsertAsync(TEntity entity, IDbTransaction transaction = null);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entityList">实体集合</param>
        /// <param name="transaction"></param>
        /// <param name="flushSize">单次执行sql语句大小,单位KB</param>
        /// <returns></returns>
        Task<bool> BatchInsertAsync(List<TEntity> entityList, IDbTransaction transaction = null, int flushSize = 2048);

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
        /// 查询
        /// </summary>
        /// <param name="expression">过滤条件</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression = null, IDbTransaction transaction = null);

        /// <summary>
        /// 执行一个命令并返回受影响的行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 执行一个命令并返回单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 查询第一条数据，不存在返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 查询单条记录，不存在返回默认值，如果存在多条记录则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);

        #region ==连接查询==

        /// <summary>
        /// 左连接
        /// </summary>
        /// <typeparam name="TEntity2"></typeparam>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> LeftJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : Entity, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity2"></typeparam>
        /// <param name="onExpression"></param>
        /// <returns></returns>
        INetSqlQueryable<TEntity, TEntity2> InnerJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : Entity, new();

        ///// <summary>
        ///// 左连接
        ///// </summary>
        ///// <typeparam name="TEntity2"></typeparam>
        ///// <typeparam name="TEntity3"></typeparam>
        ///// <returns></returns>
        //INetSqlQueryable<TEntity, TEntity2, TEntity3> LeftJoin<TEntity2, TEntity3>() where TEntity2 : Entity, new() where TEntity3 : Entity, new();

        ///// <summary>
        ///// 左连接
        ///// </summary>
        ///// <typeparam name="TEntity2"></typeparam>
        ///// <typeparam name="TEntity3"></typeparam>
        ///// <typeparam name="TEntity4"></typeparam>
        ///// <returns></returns>
        //INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4> LeftJoin<TEntity2, TEntity3, TEntity4>() where TEntity2 : Entity, new() where TEntity3 : Entity, new() where TEntity4 : Entity, new();

        #endregion

    }
}