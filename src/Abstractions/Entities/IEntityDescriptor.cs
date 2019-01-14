using System;

namespace NetSql.Abstractions.Entities
{
    /// <summary>
    /// 实体信息
    /// </summary>
    public interface IEntityDescriptor
    {
        /// <summary>
        /// 表名称
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// 列集合
        /// </summary>
        IColumnDescriptorCollection Columns { get; }

        /// <summary>
        /// 主键列
        /// </summary>
        IPrimaryKeyDescriptor PrimaryKey { get; }

        /// <summary>
        /// 是否包含软删除
        /// </summary>
        bool SoftDelete { get; }

        /// <summary>
        /// SQL语句
        /// </summary>
        EntitySql Sql { get; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        ISqlAdapter SqlAdapter { get; }
    }

    /// <summary>
    /// 实体信息
    /// </summary>
    public interface IEntityDescriptor<TEntity> : IEntityDescriptor where TEntity : IEntity, new()
    {

    }
}
