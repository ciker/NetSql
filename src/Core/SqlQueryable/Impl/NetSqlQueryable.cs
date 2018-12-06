﻿using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NetSql.Core.Entities;
using NetSql.Core.Entities.@internal;
using NetSql.Core.Enums;
using NetSql.Core.Internal;
using NetSql.Core.Pagination;
using NetSql.Core.SqlAdapter;
using NetSql.Core.SqlQueryable.Abstract;
using DatabaseType = NetSql.Core.Enums.DatabaseType;

namespace NetSql.Core.SqlQueryable.Impl
{
    internal partial class NetSqlQueryable<TEntity> : NetSqlQueryableAbstract<TEntity>, INetSqlQueryable<TEntity> where TEntity : Entity, new()
    {
        private readonly EntitySqlStatement _sqlStatement;

        public NetSqlQueryable(IDbSet<TEntity> db, ISqlAdapter sqlAdapter, EntitySqlStatement sqlStatement, Expression<Func<TEntity, bool>> whereExpression) : base(db, sqlAdapter, typeof(Func<,>).MakeGenericType(typeof(TEntity), typeof(bool)))
        {
            _sqlStatement = sqlStatement;
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

        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> expression)
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

            return Db.ExecuteScalar<TResult>(sqlBuilder.ToString());
        }

        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> expression)
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

            return Db.ExecuteScalar<TResult>(sqlBuilder.ToString());
        }

        public bool Exists()
        {
            var sqlBuilder = new StringBuilder();

            ResolveSelect(sqlBuilder, "1");

            ResolveJoin(sqlBuilder);

            ResolveWhere(sqlBuilder);

            return Db.ExecuteScalar<int>(sqlBuilder.ToString()) > 0;
        }

        public TEntity First()
        {
            var sqlBuilder = new StringBuilder();

            if (SqlAdapter.Type == DatabaseType.SqlServer)
            {
                ResolveSelect(sqlBuilder, " TOP 1 *");

                ResolveJoin(sqlBuilder);

                ResolveWhere(sqlBuilder);

                return Db.QueryFirstOrDefault<TEntity>(sqlBuilder.ToString());
            }

            SetLimit(0, 1);
            return ToList().FirstOrDefault();
        }

        public bool Delete(IDbTransaction transaction = null)
        {
            if (!HasWhere())
                throw new ArgumentNullException("where", "未指定删除条件");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(_sqlStatement.Delete);
            ResolveWhere(sqlBuilder);

            return Db.Execute(sqlBuilder.ToString(), null, transaction) > 0;
        }

        public bool Update(Expression<Func<TEntity, TEntity>> expression, IDbTransaction transaction = null)
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

            return Db.Execute(sqlBuilder.ToString(), null, transaction) > 0;
        }

    }
}
