using System;
using System.Linq;
using System.Reflection;
using NetSql.Abstractions;
using NetSql.Abstractions.Attributes;
using NetSql.Abstractions.Entities;
using NetSql.Core.Internal;

namespace NetSql.Core.Entities
{
    /// <summary>
    /// 实体描述
    /// </summary>
    public class EntityDescriptor<TEntity> : IEntityDescriptor<TEntity> where TEntity : IEntity, new()
    {
        #region ==属性==

        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// 属性列表
        /// </summary>
        public IColumnDescriptorCollection Columns { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IPrimaryKeyDescriptor PrimaryKey { get; private set; }

        /// <summary>
        /// 是否包含软删除
        /// </summary>
        public bool SoftDelete { get; }

        public EntitySql Sql { get; }

        public ISqlAdapter SqlAdapter { get; }

        #endregion

        #region ==构造器==

        public EntityDescriptor(ISqlAdapter sqlAdapter, IEntitySqlBuilder sqlBuilder)
        {
            SqlAdapter = sqlAdapter;

            EntityType = typeof(TEntity);

            PrimaryKey = new PrimaryKeyDescriptor();

            SoftDelete = EntityType.IsSubclassOfGeneric(typeof(EntityWithSoftDelete<,>));

            SetTableName();

            SetColumns();

            Sql = sqlBuilder.Build(this);
        }

        #endregion

        #region ==私有方法==

        /// <summary>
        /// 设置表名
        /// </summary>
        private void SetTableName()
        {
            var tableArr = EntityType.GetCustomAttribute<TableAttribute>(false);
            TableName = tableArr != null ? tableArr.Name : EntityType.Name;
        }

        /// <summary>
        /// 设置属性列表
        /// </summary>
        private void SetColumns()
        {
            Columns = new ColumnDescriptorCollection();

            //加载属性列表
            var properties = EntityType.GetProperties().Where(p =>
                !p.PropertyType.IsGenericType
                && (p.PropertyType == typeof(Guid) || Type.GetTypeCode(p.PropertyType) != TypeCode.Object)
                && Attribute.GetCustomAttributes(p).All(attr => attr.GetType() != typeof(IgnoreAttribute))).ToList();

            foreach (var p in properties)
            {
                var column = new ColumnDescriptorr(p);
                if (column.IsPrimaryKey)
                {
                    PrimaryKey = new PrimaryKeyDescriptor(p);
                    Columns.Insert(0, column);
                }
                else
                {
                    Columns.Add(column);
                }
            }
        }

        #endregion
    }
}
