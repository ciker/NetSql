using System.Linq.Expressions;
using Td.Fw.Data.Core.Entities;
using Td.Fw.Data.Core.Entities.@internal;

namespace Td.Fw.Data.Core.SqlQueryable
{
    internal class JoinDescriptor
    {
        /// <summary>
        /// 连接类型
        /// </summary>
        public JoinType Type { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public IEntityDescriptor EntityDescriptor { get; set; }

        /// <summary>
        /// 连接条件
        /// </summary>
        public LambdaExpression On { get; set; }
    }


    /// <summary>
    /// 连接类型
    /// </summary>
    public enum JoinType
    {
        Left,
        Inner,
        Right
    }
}
