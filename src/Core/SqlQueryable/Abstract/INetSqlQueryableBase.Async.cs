#if !NET40

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Td.Fw.Data.Core.Pagination;

namespace Td.Fw.Data.Core.SqlQueryable.Abstract
{
    public partial interface INetSqlQueryableBase<TEntity>
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        Task<IList<TResult>> ToListAsync<TResult>();

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<IList<TEntity>> ToListAsync();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<IList<TEntity>> PaginationAsync(Paging paging);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<IList<TResult>> PaginationAsync<TResult>(Paging paging);
    }

    public partial interface INetSqlQueryableBase<TEntity, out TType, in TFunc> : INetSqlQueryableBase<TEntity> where TType : INetSqlQueryableBase<TEntity> where TFunc : Expression
    {
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <returns></returns>
        Task<long> CountAsync();
    }
}

#endif