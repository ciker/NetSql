using System;

namespace Td.Fw.Data.Core.Entities
{
    /// <summary>
    /// 包含指定类型主键的实体
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class EntityBase<TKey> : Entity
    {
        public virtual TKey Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EntityBase : EntityBase<Guid>
    {

    }
}
