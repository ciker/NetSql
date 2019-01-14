using System;
using System.Linq;
using System.Reflection;
using NetSql.Abstractions.Attributes;
using NetSql.Abstractions.Entities;

namespace NetSql.Core.Entities
{
    public class ColumnDescriptorr : IColumnDescriptor
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; }

        public ColumnDescriptorr(PropertyInfo property)
        {
            if (property == null)
                return;

            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            Name = columnAttribute != null ? columnAttribute.Name : property.Name;
            PropertyInfo = property;

            IsPrimaryKey = Attribute.GetCustomAttributes(property).Any(attr => attr.GetType() == typeof(KeyAttribute));

            if (!IsPrimaryKey)
            {
                IsPrimaryKey = property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
