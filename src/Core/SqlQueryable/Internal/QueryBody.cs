using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetSql.Abstractions;
using NetSql.Abstractions.Enums;
using NetSql.Abstractions.Pagination;
using NetSql.Core.Internal;

namespace NetSql.Core.SqlQueryable.Internal
{
    /// <summary>
    /// 查询主体信息
    /// </summary>
    internal class QueryBody
    {
        private readonly ISqlAdapter _sqlAdapter;

        public QueryBody(ISqlAdapter sqlAdapter)
        {
            _sqlAdapter = sqlAdapter;
        }

        #region ==属性==

        public Type WhereDelegateType { get; set; }

        /// <summary>
        /// 表连接信息
        /// </summary>
        public List<QueryJoinDescriptor> JoinDescriptors { get; } = new List<QueryJoinDescriptor>();

        /// <summary>
        /// 排序
        /// </summary>
        public List<Sort> Sorts { get; } = new List<Sort>();

        /// <summary>
        /// 查询的列
        /// </summary>
        public LambdaExpression Select { get; set; }

        /// <summary>
        /// 过滤条件
        /// </summary>
        public LambdaExpression Where { get; set; }

        /// <summary>
        /// 更新表达式
        /// </summary>
        public LambdaExpression Update { get; set; }

        /// <summary>
        /// 查询最大值表达式
        /// </summary>
        public LambdaExpression Max { get; set; }

        /// <summary>
        /// 查询最小值表达式
        /// </summary>
        public LambdaExpression Min { get; set; }

        /// <summary>
        /// 跳过行数
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// 取行数
        /// </summary>
        public int Take { get; set; }

        #endregion

        #region ==方法==

        public void SetWhere(LambdaExpression whereExpression)
        {
            if (Where == null)
                Where = whereExpression;
            else
            {
                var exp = Expression.AndAlso(Where.Body, whereExpression.Body);
                var parameterList = new List<ParameterExpression>();
                foreach (var descriptor in JoinDescriptors)
                {
                    parameterList.Add(Expression.Parameter(descriptor.EntityDescriptor.EntityType, descriptor.Alias));
                }

                Where = Expression.Lambda(WhereDelegateType, exp, parameterList);
            }
        }

        public void SetOrderBy(LambdaExpression expression, SortType sortType = SortType.Asc)
        {
            if (expression == null)
                return;

            if (expression == null || !(expression.Body is MemberExpression memberExpression) || memberExpression.Expression.NodeType != ExpressionType.Parameter)
                throw new ArgumentException("排序列无效");

            Sorts.Add(new Sort(GetColumnName(memberExpression), sortType));
        }

        public void SetOrderBy(Sort sort)
        {
            if (sort == null)
                return;

            foreach (var des in JoinDescriptors)
            {
                foreach (var column in des.EntityDescriptor.Columns)
                {
                    if (column.Name.Equals(sort.OrderBy, StringComparison.OrdinalIgnoreCase) ||
                        column.PropertyInfo.Name.Equals(sort.OrderBy, StringComparison.OrdinalIgnoreCase))
                    {
                        Sorts.Add(sort);
                        break;
                    }
                }
            }
        }

        public void SetSelect(LambdaExpression selectExpression)
        {
            Select = selectExpression;
        }

        public void SetLimit(int skip, int take)
        {
            Skip = skip < 0 ? 0 : skip;
            Take = take < 0 ? 0 : take;
        }

        public string GetColumnName(MemberExpression exp)
        {
            var descriptor = JoinDescriptors.First(m => m.EntityDescriptor.EntityType == exp.Expression.Type);
            var col = descriptor.EntityDescriptor.Columns.FirstOrDefault(m => m.PropertyInfo.Name.Equals(exp.Member.Name));

            Check.NotNull(col, nameof(col), $"({exp.Member.Name})列不存在");

            return $"{_sqlAdapter.AppendQuote(descriptor.Alias)}.{_sqlAdapter.AppendQuote(col.Name)}";
        }

        #endregion
    }
}
