using System;
using System.Collections.Generic;
using NetSql.Enums;

namespace NetSql.Entities
{
    internal interface IEntityDescriptor
    {
        #region ==属性==

        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// 表名称
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 属性列表
        /// </summary>
        List<ColumnDescriptor> Columns { get; }

        /// <summary>
        /// 主键类型
        /// </summary>
        PrimaryKeyType PrimaryKeyType { get; }

        /// <summary>
        /// 主键属性
        /// </summary>
        ColumnDescriptor PrimaryKey { get; }

        #endregion

        /// <summary>
        /// 根据属性名称获取指定列
        /// </summary>
        /// <param name="propertyName">列对应的属性名称</param>
        /// <returns></returns>
        ColumnDescriptor GetColumnByPropertyName(string propertyName);
    }
}
