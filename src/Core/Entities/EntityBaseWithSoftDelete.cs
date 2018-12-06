using System;

namespace NetSql.Core.Entities
{
    /// <summary>
    /// 包含指定类型主键的软删除实体
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TDeletorKey">删除人主键类型</typeparam>
    public class EntityBaseWithSoftDelete<TKey, TDeletorKey> : EntityBase<TKey>
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


    public class EntityBaseWithSoftDelete : EntityBaseWithSoftDelete<Guid, Guid>
    {

    }
}
