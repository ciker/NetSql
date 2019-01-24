using System;
using System.Linq;
using System.Linq.Expressions;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Core.Internal;

namespace NetSql.Core.SqlQueryable.Internal
{
    /// <summary>
    /// 查询表连接信息集合
    /// </summary>
    internal class QueryJoinCollection : CollectionAbstract<QueryJoinDescriptor>
    {
        private readonly ISqlAdapter _sqlAdapter;

        public QueryJoinCollection(ISqlAdapter sqlAdapter)
        {
            _sqlAdapter = sqlAdapter;
        }

        public QueryJoinDescriptor Get<T>() where T : IEntity
        {
            return Collection.First(m => m.EntityDescriptor.EntityType == typeof(T));
        }

        public QueryJoinDescriptor Get(Type entityType)
        {
            return Collection.First(m => m.EntityDescriptor.EntityType == entityType);
        }

        public QueryJoinDescriptor Get(MemberExpression exp)
        {
            return Get(exp.Expression.Type);
        }

        /// <summary>
        /// 根据成员表达式获取列名称
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string GetColumnName(MemberExpression exp)
        {
            var descriptor = Get(exp);
            var col = descriptor.EntityDescriptor.Columns.FirstOrDefault(m => m.PropertyInfo.Name.Equals(exp.Member.Name));

            Check.NotNull(col, nameof(col), $"({exp.Member.Name})列不存在");

            if (Count > 1)
                return $"{_sqlAdapter.AppendQuote(descriptor.Alias)}.{_sqlAdapter.AppendQuote(col.Name)}";

            return $"{_sqlAdapter.AppendQuote(col.Name)}";
        }
    }
}
