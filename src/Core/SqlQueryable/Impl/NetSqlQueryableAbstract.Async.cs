#if !NET40

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSql.Core.Entities;
using NetSql.Core.Pagination;
using NetSql.Core.SqlQueryable.Abstract;

namespace NetSql.Core.SqlQueryable.Impl
{
    internal abstract partial class NetSqlQueryableAbstract<TEntity> : INetSqlQueryableBase<TEntity> where TEntity : Entity, new()
    {
        public async Task<IList<TResult>> ToListAsync<TResult>()
        {
            return (await Db.QueryAsync<TResult>(ToSql())).ToList();
        }

        public async Task<IList<TEntity>> ToListAsync()
        {
            return (await Db.QueryAsync<TEntity>(ToSql())).ToList();
        }

        public Task<long> CountAsync()
        {
            var sqlBuilder = new StringBuilder();

            ResolveSelect(sqlBuilder, "COUNT(0)");

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            return Db.ExecuteScalarAsync<long>(sqlBuilder.ToString());
        }

        public async Task<IList<TEntity>> PaginationAsync(Paging paging)
        {
            SetLimit(paging.Skip, paging.Size);
            paging.TotalCount = await CountAsync();
            return await ToListAsync();
        }

        public async Task<IList<TResult>> PaginationAsync<TResult>(Paging paging)
        {
            SetLimit(paging.Skip, paging.Size);
            paging.TotalCount = await CountAsync();
            return await ToListAsync<TResult>();
        }
    }
}

#endif