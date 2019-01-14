﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.SqlQueryable;

namespace NetSql.Abstractions
{
    /// <summary>
    /// 数据集
    /// </summary>
    public interface IDbSet<TEntity> where TEntity : IEntity, new()
    {
        #region ==属性==

        /// <summary>
        /// 数据库上下文
        /// </summary>
        IDbContext DbContext { get; }

        /// <summary>
        /// 实体信息
        /// </summary>
        IEntityDescriptor EntityDescriptor { get; }

        #endregion

        #region ==Insert==

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Insert(TEntity entity);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> InsertAsync(TEntity entity);

        #endregion

        #region ==BatchInsert==

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entityList">实体集合</param>
        /// <param name="flushSize">单次执行sql语句大小,单位KB</param>
        /// <returns></returns>
        bool BatchInsert(List<TEntity> entityList, int flushSize = 2048);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entityList">实体集合</param>
        /// <param name="flushSize">单次执行sql语句大小,单位KB</param>
        /// <returns></returns>
        Task<bool> BatchInsertAsync(List<TEntity> entityList, int flushSize = 2048);

        #endregion

        #region ==Delete==

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(dynamic id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(dynamic id);

        #endregion

        #region ==SoftDelete==

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deletor">删除人</param>
        /// <returns></returns>
        bool SoftDelete(dynamic id, dynamic deletor);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deletor">删除人</param>
        /// <returns></returns>
        Task<bool> SoftDeleteAsync(dynamic id, dynamic deletor);

        #endregion

        #region ==Update==

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Update(TEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(TEntity entity);

        #endregion

        #region ==Get==

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(dynamic id);

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(dynamic id);

        #endregion

        #region ==Execute==

        /// <summary>
        /// 执行一个命令并返回受影响的行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>

        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        int Execute(string sql, object param = null, CommandType? commandType = null);

        /// <summary>
        /// 执行一个命令并返回受影响的行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null);

        #endregion

        #region ==ExecuteScalar==

        /// <summary>
        /// 执行一个命令并返回单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null);

        /// <summary>
        /// 执行一个命令并返回单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null);

        #endregion

        #region ==QueryFirstOrDefault==

        /// <summary>
        /// 查询第一条数据，不存在返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        T QueryFirstOrDefault<T>(string sql, object param = null, CommandType? commandType = null);

        /// <summary>
        /// 查询第一条数据，不存在返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType? commandType = null);

        #endregion

        #region ==QuerySingleOrDefault==

        /// <summary>
        /// 查询单条记录，不存在返回默认值，如果存在多条记录则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        T QuerySingleOrDefault<T>(string sql, object param = null, CommandType? commandType = null);

        /// <summary>
        /// 查询单条记录，不存在返回默认值，如果存在多条记录则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, CommandType? commandType = null);

        #endregion

        #region ==Query==

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string sql, object param = null, CommandType? commandType = null);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="expression">过滤条件</param>
        /// <returns></returns>
        INetSqlQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression = null);

        #endregion

        #region ==Pagination==

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

        #endregion
    }
}
