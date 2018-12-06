using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetSql.Core.Enums;
using NetSql.Core.Mapper;

namespace NetSql.Core.Entities.@internal
{
    /// <summary>
    /// 实体描述
    /// </summary>
    internal class EntityDescriptor<TEntity> : IEntityDescriptor where TEntity : Entity, new()
    {
        #region ==属性==

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 属性列表
        /// </summary>
        public List<ColumnDescriptor> Columns { get; private set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public PrimaryKeyType PrimaryKeyType { get; private set; }

        /// <summary>
        /// 主键属性
        /// </summary>
        public ColumnDescriptor PrimaryKey { get; private set; }

        /// <summary>
        /// 是否包含软删除
        /// </summary>
        public bool SoftDelete { get; }

        public ColumnDescriptor GetColumnByPropertyName(string name)
        {
            return Columns.FirstOrDefault(m => m.PropertyInfo.Name.Equals(name));
        }

        #endregion

        #region ==构造器==

        public EntityDescriptor()
        {
            EntityType = typeof(TEntity);

            SoftDelete = EntityType.IsSubclassOf(typeof(EntityBaseWithSoftDelete<,>));

            SetTableName();

            SetColumns();
        }

        #endregion

        #region ==私有方法==


        /// <summary>
        /// 设置表名
        /// </summary>
        private void SetTableName()
        {
#if !NET40
            var tableArr = EntityType.GetCustomAttribute<TableAttribute>(false);
#else
            var tableArr = (TableAttribute)Attribute.GetCustomAttribute(EntityType, typeof(TableAttribute), false);
#endif
            TableName = tableArr != null ? tableArr.Name : EntityType.Name;
        }

        /// <summary>
        /// 设置属性列表
        /// </summary>
        private void SetColumns()
        {
            Columns = new List<ColumnDescriptor>();

            //加载属性列表
            var properties = EntityType.GetProperties().Where(p =>
                !p.PropertyType.IsGenericType
                && (p.PropertyType == typeof(Guid) || Type.GetTypeCode(p.PropertyType) != TypeCode.Object)
                && Attribute.GetCustomAttributes(p).All(attr => attr.GetType() != typeof(IgnoreAttribute))).ToList();

            var primaryKey = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            if (primaryKey != null)
                Columns.Add(Property2Column(primaryKey));

            foreach (var p in properties)
            {
                if (primaryKey != p)
                    Columns.Add(Property2Column(p));
            }
        }

        /// <summary>
        /// 设置主键类型
        /// </summary>
        private ColumnDescriptor Property2Column(PropertyInfo property)
        {
#if !NET40

            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
#else
            var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property,typeof(ColumnAttribute),false);
#endif

            var col = new ColumnDescriptor
            {
                Name = columnAttribute != null ? columnAttribute.Name : property.Name,
                PropertyInfo = property,
                PropertyType = property.PropertyType
            };

            var isPrimaryKey = Attribute.GetCustomAttributes(property).Any(attr => attr.GetType() == typeof(KeyAttribute));

            if (!isPrimaryKey)
            {
                isPrimaryKey = property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
            }

            if (isPrimaryKey)
            {
                if (property.PropertyType == typeof(int))
                {
                    PrimaryKeyType = PrimaryKeyType.Int;
                }
                else if (property.PropertyType == typeof(long))
                {
                    PrimaryKeyType = PrimaryKeyType.Long;
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    PrimaryKeyType = PrimaryKeyType.Guid;
                }
                else
                {
                    throw new ArgumentException("无效的主键类型", nameof(PrimaryKey.PropertyType));
                }

                col.IsPrimaryKey = true;

                PrimaryKey = col;
            }

            return col;
        }

        #endregion
    }
}
