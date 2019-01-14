using System;
using NetSql.Abstractions.Entities;

namespace NetSql.Core.Entities
{
    /// <summary>
    /// 包含指定类型主键的实体
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class Entity<TKey> : IEntity
    {
        public virtual TKey Id { get; set; }
    }

    /// <summary>
    /// 主键类型为GUID的实体
    /// </summary>
    public class Entity : Entity<Guid>
    {

    }

    /// <summary>
    /// 包含指定类型主键的软删除实体
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TDeletorKey">删除人主键类型</typeparam>
    public class EntityWithSoftDelete<TKey, TDeletorKey> : Entity<TKey>
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime DeletedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 删除人
        /// </summary>
        public TDeletorKey Deletor { get; set; } = default(TDeletorKey);
    }

    /// <summary>
    /// 主键类型GUID的软删除实体
    /// </summary>
    public class EntityWithSoftDelete : EntityWithSoftDelete<Guid, Guid>
    {

    }
}
