#if !NET40

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using NetSql.Core.Entities;

namespace NetSql.Core
{
    public partial interface IDbSet<TEntity> : IDbSet where TEntity : Entity, new()
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
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deletor">删除人</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> SoftDeleteAsync(dynamic id, dynamic deletor, IDbTransaction transaction = null);

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
    }
}
#endif