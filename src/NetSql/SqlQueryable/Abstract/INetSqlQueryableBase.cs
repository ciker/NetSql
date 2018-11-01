using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetSql.Pagination;

namespace NetSql.SqlQueryable.Abstract
{
    public interface INetSqlQueryableBase<TEntity>
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        Task<IList<TResult>> ToList<TResult>();

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<IList<TEntity>> ToList();

        /// <summary>
        /// 获取Sql语句
        /// </summary>
        /// <returns></returns>
        string ToSql();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<IList<TEntity>> Pagination(Paging paging);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<IList<TResult>> Pagination<TResult>(Paging paging);
    }

    public interface INetSqlQueryableBase<TEntity, out TType, in TFunc> : INetSqlQueryableBase<TEntity> where TType : INetSqlQueryableBase<TEntity> where TFunc : Expression
    {
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="expression">过滤条件</param>
        /// <returns></returns>
        TType Where(TFunc expression);

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="isAdd">是否添加</param>
        /// <param name="expression">条件</param>
        /// <returns></returns>
        TType WhereIf(bool isAdd, TFunc expression);

        /// <summary>
        /// 升序
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        TType OrderBy(string colName);

        /// <summary>
        /// 降序
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        TType OrderByDescending(string colName);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        TType Order(Sort sort);

        /// <summary>
        /// 限制
        /// </summary>
        /// <param name="skip">跳过前几条数据</param>
        /// <param name="take">取前几条数据</param>
        /// <returns></returns>
        TType Limit(int skip, int take);

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <returns></returns>
        Task<long> Count();
    }
}
