using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.Enums;
using NetSql.Abstractions.Pagination;
using NetSql.Abstractions.SqlQueryable;
using NetSql.Core.Expressions;
using NetSql.Core.Internal;

namespace NetSql.Core.SqlQueryable
{
    internal abstract class NetSqlQueryableAbstract<TEntity> : INetSqlQueryableBase<TEntity> where TEntity : IEntity, new()
    {
        protected readonly IDbSet<TEntity> Db;
        protected readonly ISqlAdapter SqlAdapter;
        protected readonly IExpressionResolve ExpressionResolve;

        private readonly Type _detegateType;//用于连接where表达式

        protected readonly QueryableBody QueryableBody;
        protected JoinCollection JoinCollection;

        protected NetSqlQueryableAbstract(IDbSet<TEntity> dbSet, Type detegateType, JoinCollection joinCollection = null, QueryableBody queryableBody = null)
        {
            Db = dbSet;
            SqlAdapter = dbSet.DbContext.Options.SqlAdapter;
            _detegateType = detegateType;

            if (joinCollection == null)
            {
                JoinCollection = new JoinCollection(SqlAdapter);
                SetJoin(new JoinDescriptor
                {
                    Alias = "T1",
                    EntityDescriptor = dbSet.EntityDescriptor,
                });
            }
            else
            {
                JoinCollection = joinCollection;
            }


            QueryableBody = queryableBody ?? new QueryableBody();

            ExpressionResolve = new ExpressionResolve(SqlAdapter, JoinCollection);
        }

        #region ==ToList==

        public IList<TResult> ToList<TResult>()
        {
            return (Db.Query<TResult>(ToSql())).ToList();
        }

        public async Task<IList<TResult>> ToListAsync<TResult>()
        {
            return (await Db.QueryAsync<TResult>(ToSql())).ToList();
        }

        public IList<TEntity> ToList()
        {
            return (Db.Query<TEntity>(ToSql())).ToList();
        }

        public async Task<IList<TEntity>> ToListAsync()
        {
            return (await Db.QueryAsync<TEntity>(ToSql())).ToList();
        }

        #endregion

        #region ==Count==

        private string CountSqlBuild()
        {
            var sqlBuilder = new StringBuilder();

            ResolveSelect(sqlBuilder, "COUNT(0)");

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            return sqlBuilder.ToString();
        }

        public long Count()
        {
            return Db.ExecuteScalar<long>(CountSqlBuild());
        }

        public Task<long> CountAsync()
        {
            return Db.ExecuteScalarAsync<long>(CountSqlBuild());
        }

        #endregion

        #region ==Pagination==

        public IList<TEntity> Pagination(Paging paging)
        {
            SetLimit(paging.Skip, paging.Size);
            paging.TotalCount = Count();
            return ToList();
        }

        public async Task<IList<TEntity>> PaginationAsync(Paging paging)
        {
            SetLimit(paging.Skip, paging.Size);
            paging.TotalCount = await CountAsync();
            return await ToListAsync();
        }

        public IList<TResult> Pagination<TResult>(Paging paging)
        {
            SetLimit(paging.Skip, paging.Size);
            paging.TotalCount = Count();
            return ToList<TResult>();
        }

        public async Task<IList<TResult>> PaginationAsync<TResult>(Paging paging)
        {
            SetLimit(paging.Skip, paging.Size);
            paging.TotalCount = await CountAsync();
            return await ToListAsync<TResult>();
        }

        #endregion

        public string ToSql()
        {
            //分页查询
            if (QueryableBody.Take > 0)
            {
                var where = ExpressionResolve.ResolveWhere(QueryableBody.Where);
                var sort = ExpressionResolve.ResolveOrder(QueryableBody.Sorts);
                var select = ExpressionResolve.ResolveSelect(QueryableBody.Select);
                var table = ExpressionResolve.ResolveJoin();

                #region ==SqlServer分页需要指定排序==

                if (SqlAdapter.SqlDialect == SqlDialect.SqlServer && sort.IsNull())
                {
                    var first = JoinCollection.First();
                    if (first.EntityDescriptor.PrimaryKey.IsNo())
                    {
                        throw new ArgumentNullException("OrderBy", "SqlServer数据库没有主键的表需要指定排序字段才可以分页查询");
                    }

                    if (JoinCollection.Count > 1)
                    {
                        sort = $"{SqlAdapter.AppendQuote(first.Alias)}.{SqlAdapter.AppendQuote(first.EntityDescriptor.PrimaryKey.Name)}";
                    }
                    else
                    {
                        sort = JoinCollection.First().EntityDescriptor.PrimaryKey.Name;
                    }
                }

                #endregion

                return SqlAdapter.GeneratePagingSql(select, table, where, sort, QueryableBody.Skip, QueryableBody.Take);
            }

            var sqlBuilder = new StringBuilder();

            ResolveSelect(sqlBuilder);

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            ResolveOrder(sqlBuilder);

            return sqlBuilder.ToString();
        }

        protected void SetWhere(LambdaExpression wherExpression)
        {
            if (QueryableBody.Where == null)
                QueryableBody.Where = wherExpression;
            else
            {
                var exp = Expression.AndAlso(QueryableBody.Where.Body, wherExpression.Body);
                var parameterList = new List<ParameterExpression>();
                foreach (var descriptor in JoinCollection)
                {
                    parameterList.Add(Expression.Parameter(descriptor.EntityDescriptor.EntityType, descriptor.Alias));
                }

                QueryableBody.Where = Expression.Lambda(_detegateType, exp, parameterList);
            }
        }

        protected void SetOrderBy(LambdaExpression expression, SortType sortType = SortType.Asc)
        {
            if (expression == null)
                return;

            if (expression == null || !(expression.Body is MemberExpression memberExpression) || memberExpression.Expression.NodeType != ExpressionType.Parameter)
                throw new ArgumentException("排序列无效");

            QueryableBody.Sorts.Add(new Sort(JoinCollection.GetColumn(memberExpression), sortType));
        }

        protected void SetOrderBy(Sort sort)
        {
            if (sort == null)
                return;

            foreach (var des in JoinCollection)
            {
                foreach (var column in des.EntityDescriptor.Columns)
                {
                    if (column.Name.Equals(sort.OrderBy, StringComparison.OrdinalIgnoreCase) ||
                        column.PropertyInfo.Name.Equals(sort.OrderBy, StringComparison.OrdinalIgnoreCase))
                    {
                        QueryableBody.Sorts.Add(sort);
                        break;
                    }
                }
            }
        }

        protected void SetSelect(LambdaExpression selectExpression)
        {
            QueryableBody.Select = selectExpression;
        }

        protected void SetJoin(JoinDescriptor descriptor)
        {
            JoinCollection.Add(descriptor);
        }

        protected void SetLimit(int skip, int take)
        {
            QueryableBody.Skip = skip < 0 ? 0 : skip;
            QueryableBody.Take = take < 0 ? 0 : take;
        }

        protected void ResolveSelect(StringBuilder sqlBuilder, string select)
        {
            Check.NotNull(select, nameof(select));

            sqlBuilder.AppendFormat("SELECT {0} ", select);
        }

        protected void ResolveSelect(StringBuilder sqlBuilder)
        {
            sqlBuilder.Append("SELECT ");
            ExpressionResolve.ResolveSelect(sqlBuilder, QueryableBody.Select);
        }

        protected void ResolveJoin(StringBuilder sqlBuilder)
        {
            sqlBuilder.Append(" FROM ");
            ExpressionResolve.ResolveJoin(sqlBuilder);
        }

        protected void ResolveWhere(StringBuilder sqlBuilder)
        {
            var where = ExpressionResolve.ResolveWhere(QueryableBody.Where);
            if (where.NotNull())
                sqlBuilder.AppendFormat(" WHERE {0}", where);
        }

        protected void ResolveOrder(StringBuilder sqlBuilder)
        {
            var sort = ExpressionResolve.ResolveOrder(QueryableBody.Sorts);
            if (sort.NotNull())
                sqlBuilder.AppendFormat(" ORDER BY {0}", sort);
        }

        protected bool HasWhere()
        {
            var where = ExpressionResolve.ResolveWhere(QueryableBody.Where);

            return where.NotNull();
        }
    }
}
