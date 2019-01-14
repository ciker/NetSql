using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.Enums;
using NetSql.Abstractions.Pagination;
using NetSql.Abstractions.SqlQueryable;
using NetSql.Core.Internal;

namespace NetSql.Core.SqlQueryable
{
    internal class NetSqlQueryable<TEntity> : NetSqlQueryableAbstract<TEntity>, INetSqlQueryable<TEntity> where TEntity : IEntity, new()
    {
        private readonly EntitySql _sql;

        public NetSqlQueryable(IDbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> whereExpression) : base(dbSet, typeof(Func<,>).MakeGenericType(typeof(TEntity), typeof(bool)))
        {
            _sql = dbSet.EntityDescriptor.Sql;
            Where(whereExpression);
        }

        public INetSqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            SetWhere(expression);
            return this;
        }

        public INetSqlQueryable<TEntity> WhereIf(bool isAdd, Expression<Func<TEntity, bool>> expression)
        {
            if (isAdd)
                Where(expression);

            return this;
        }

        public INetSqlQueryable<TEntity> OrderBy(string colName)
        {
            return Order(new Sort(colName));
        }

        public INetSqlQueryable<TEntity> OrderByDescending(string colName)
        {
            return Order(new Sort(colName, SortType.Desc));
        }

        public INetSqlQueryable<TEntity> Order(Sort sort)
        {
            SetOrderBy(sort);
            return this;
        }

        public INetSqlQueryable<TEntity> Limit(int skip, int take)
        {
            SetLimit(skip, take);
            return this;
        }

        public INetSqlQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            SetOrderBy(expression);
            return this;
        }

        public INetSqlQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            SetOrderBy(expression, SortType.Desc);
            return this;
        }

        public INetSqlQueryable<TEntity> Select<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            SetSelect(expression);
            return this;
        }

        #region ==Exist==

        private string ExistsSqlBuild()
        {
            var sqlBuilder = new StringBuilder();

            ResolveSelect(sqlBuilder, "1");

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            return sqlBuilder.ToString();
        }

        public bool Exists()
        {
            return Db.ExecuteScalar<int>(ExistsSqlBuild()) > 0;
        }

        public async Task<bool> ExistsAsync()
        {
            return await Db.ExecuteScalarAsync<int>(ExistsSqlBuild()) > 0;
        }

        #endregion

        #region ==First==

        public TEntity First()
        {
            var sqlBuilder = new StringBuilder();

            if (SqlAdapter.SqlDialect == SqlDialect.SqlServer)
            {
                ResolveSelect(sqlBuilder, " TOP 1 *");

                ResolveJoin(sqlBuilder);

                ResolveWhere(sqlBuilder);

                return Db.QueryFirstOrDefault<TEntity>(sqlBuilder.ToString());
            }

            SetLimit(0, 1);
            return ToList().FirstOrDefault();
        }

        public async Task<TEntity> FirstAsync()
        {
            var sqlBuilder = new StringBuilder();

            if (SqlAdapter.SqlDialect == SqlDialect.SqlServer)
            {
                ResolveSelect(sqlBuilder, " TOP 1 *");

                ResolveJoin(sqlBuilder);

                ResolveWhere(sqlBuilder);

                return await Db.QueryFirstOrDefaultAsync<TEntity>(sqlBuilder.ToString());
            }

            SetLimit(0, 1);
            return (await ToListAsync()).FirstOrDefault();
        }

        #endregion

        #region ==Delete==

        private string DeleteSqlBuild()
        {
            if (!HasWhere())
                throw new ArgumentNullException("where", "未指定删除条件");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(_sql.Delete);
            ResolveWhere(sqlBuilder);

            return sqlBuilder.ToString();
        }

        public bool Delete()
        {
            Db.Execute(DeleteSqlBuild());
            return true;
        }

        public Task<bool> DeleteAsync()
        {
            DeleteWithAffectedNumAsync();
            return Task.FromResult(true);
        }

        public Task<int> DeleteWithAffectedNumAsync()
        {
            return Db.ExecuteAsync(DeleteSqlBuild());
        }

        #endregion

        #region ==Max==

        private string MaxSqlBuild<TResult>(Expression<Func<TEntity, TResult>> expression)
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

            return sqlBuilder.ToString();
        }

        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return Db.ExecuteScalar<TResult>(MaxSqlBuild(expression));

        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return Db.ExecuteScalarAsync<TResult>(MaxSqlBuild(expression));
        }

        #endregion

        #region ==Min==

        private string MinSqlBuild<TResult>(Expression<Func<TEntity, TResult>> expression)
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

            return sqlBuilder.ToString();
        }

        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return Db.ExecuteScalar<TResult>(MinSqlBuild(expression));
        }

        public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            return Db.ExecuteScalarAsync<TResult>(MinSqlBuild(expression));
        }

        #endregion

        #region ==Update==

        private string UpdateSqlBuild(Expression<Func<TEntity, TEntity>> expression)
        {
            if (!HasWhere())
                throw new ArgumentNullException("where", "未指定删除条件");

            var updateSql = ExpressionResolve.ResolveWhere(expression);
            Check.NotNull(updateSql, nameof(updateSql), "生成更新sql异常");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(_sql.Update);
            sqlBuilder.Append(" ");
            sqlBuilder.Append(updateSql);
            ResolveWhere(sqlBuilder);

            return sqlBuilder.ToString();
        }

        public bool Update(Expression<Func<TEntity, TEntity>> expression)
        {
            Db.Execute(UpdateSqlBuild(expression));
            return true;
        }

        public Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> expression)
        {
            UpdateWithAffectedNumAsync(expression);
            return Task.FromResult(true);
        }

        public Task<int> UpdateWithAffectedNumAsync(Expression<Func<TEntity, TEntity>> expression)
        {
            return Db.ExecuteAsync(UpdateSqlBuild(expression));
        }

        #endregion

    }
}
