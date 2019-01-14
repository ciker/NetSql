using NetSql.Abstractions.Entities;
using NetSql.Core.Internal;

namespace NetSql.Core.Entities
{
    /// <summary>
    /// 列信息集合
    /// </summary>
    public class ColumnDescriptorCollection : CollectionAbstract<IColumnDescriptor>, IColumnDescriptorCollection
    {
    }
}
