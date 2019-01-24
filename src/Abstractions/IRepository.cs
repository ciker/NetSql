using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Abstractions.Entities;

namespace NetSql.Abstractions
{
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : IEntity, new()
    {
        #region ==Exists==

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where);

        #endregion

        #region ==Add==

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Add(TEntity entity);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> AddAsync(TEntity entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns></returns>
        bool Add(List<TEntity> list);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns></returns>
        Task<bool> AddAsync(List<TEntity> list);

        #endregion

        #region ==Delete==

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Remove(dynamic id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(dynamic id);

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

        /// <summary>
        /// 根据表达式查询单条记录
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// 根据表达式查询单条记录
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);

        #endregion

        #region ==GetAll==

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        IList<TEntity> GetAll();

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        Task<IList<TEntity>> GetAllAsync();

        #endregion
    }
}
