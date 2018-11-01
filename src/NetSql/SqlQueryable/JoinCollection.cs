using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetSql.Entities;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.SqlQueryable
{
    internal class JoinCollection : IEnumerable<JoinDescriptor>
    {
        private readonly ISqlAdapter _sqlAdapter;
        private readonly List<JoinDescriptor> _joinDescriptors = new List<JoinDescriptor>();

        public JoinCollection(ISqlAdapter sqlAdapter)
        {
            _sqlAdapter = sqlAdapter;
        }

        public JoinDescriptor this[int index]
        {
            get => _joinDescriptors[index];
            set => _joinDescriptors[index] = value;
        }

        public int Count => _joinDescriptors.Count;

        public void Add(JoinDescriptor descriptor)
        {
            _joinDescriptors.Add(descriptor);
        }

        public JoinDescriptor Get<T>() where T : Entity
        {
            return _joinDescriptors.First(m => m.EntityDescriptor.EntityType == typeof(T));
        }

        public JoinDescriptor Get(Type entityType)
        {
            return _joinDescriptors.First(m => m.EntityDescriptor.EntityType == entityType);
        }

        public JoinDescriptor Get(MemberExpression exp)
        {
            return Get(exp.Expression.Type);
        }

        public string GetColumn(MemberExpression exp)
        {
            var descriptor = Get(exp);
            var col = descriptor.EntityDescriptor.GetColumnByPropertyName(exp.Member.Name);
            Check.NotNull(col, nameof(col), $"({exp.Member.Name})列不存在");

            if (Count > 1)
                return $"{_sqlAdapter.AppendQuote(descriptor.Alias)}.{_sqlAdapter.AppendQuote(col.Name)}";

            return $"{_sqlAdapter.AppendQuote(col.Name)}";
        }

        public IEnumerator<JoinDescriptor> GetEnumerator()
        {
            return _joinDescriptors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
