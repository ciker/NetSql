﻿#if !NET40

using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NetSql.Core.Entities;
using NetSql.Core.Enums;
using NetSql.Core.Internal;
using NetSql.Core.SqlQueryable.Abstract;

namespace NetSql.Core.SqlQueryable.Impl
{
    internal partial class NetSqlQueryable<TEntity> : NetSqlQueryableAbstract<TEntity>, INetSqlQueryable<TEntity> where TEntity : Entity, new()
    {
        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            Check.NotNull(expression, nameof(expression), "未指定求最大值的列");

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("无法解析表达式", nameof(expression));

            var sqlBuilder = new StringBuilder();
            var select = $"MAX({ JoinCollection.GetColumn(memberExpression)})";

            ResolveSelect(sqlBuilder, select);

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            return Db.ExecuteScalarAsync<TResult>(sqlBuilder.ToString());
        }

        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            Check.NotNull(expression, nameof(expression), "未指定求最大值的列");

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("无法解析表达式", nameof(expression));

            var sqlBuilder = new StringBuilder();
            var select = $"MIN({ JoinCollection.GetColumn(memberExpression)})";

            ResolveSelect(sqlBuilder, select);

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            return Db.ExecuteScalarAsync<TResult>(sqlBuilder.ToString());
        }

        public async Task<bool> ExistsAsync()
        {
            var sqlBuilder = new StringBuilder();

            ResolveSelect(sqlBuilder, "1");

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            return await Db.ExecuteScalarAsync<int>(sqlBuilder.ToString()) > 0;
        }

        public async Task<TEntity> FirstAsync()
        {
            var sqlBuilder = new StringBuilder();

            if (SqlAdapter.Type == DatabaseType.SqlServer)
            {
                ResolveSelect(sqlBuilder, " TOP 1 *");

                ResolveJoin(sqlBuilder);

                ResolveWhere(sqlBuilder);

                return await Db.QueryFirstOrDefaultAsync<TEntity>(sqlBuilder.ToString());
            }

            SetLimit(0, 1);
            return (await ToListAsync()).FirstOrDefault();
        }

        public async Task<bool> DeleteAsync(IDbTransaction transaction = null)
        {
            if (!HasWhere())
                throw new ArgumentNullException("where", "未指定删除条件");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(_sqlStatement.Delete);
            ResolveWhere(sqlBuilder);

            return await Db.ExecuteAsync(sqlBuilder.ToString(), null, transaction) > 0;
        }

        public async Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> expression, IDbTransaction transaction = null)
        {
            if (!HasWhere())
                throw new ArgumentNullException("where", "未指定删除条件");

            var updateSql = ExpressionResolve.ResolveWhere(expression);
            Check.NotNull(updateSql, nameof(updateSql), "生成更新sql异常");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(_sqlStatement.Update);
            sqlBuilder.Append(" ");
            sqlBuilder.Append(updateSql);
            ResolveWhere(sqlBuilder);

            return await Db.ExecuteAsync(sqlBuilder.ToString(), null, transaction) > 0;
        }

    }
}

#endif
