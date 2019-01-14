using System.Collections.Generic;

namespace NetSql.Abstractions.Entities
{
    /// <summary>
    /// 表集合
    /// </summary>
    public interface IEntityDescriptorCollection : IList<IEntityDescriptor>
    {
        /// <summary>
        /// 获取实体信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IEntityDescriptor Get<TEntity>() where TEntity : IEntity, new();
    }
}
