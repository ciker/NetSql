using System.Data;
using Microsoft.Extensions.Logging;
using NetSql.Abstractions.Entities;

namespace NetSql.Abstractions
{
    /// <summary>
    /// 数据库配置项
    /// </summary>
    public interface IDbContextOptions
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        ISqlAdapter SqlAdapter { get; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 实体信息集合
        /// </summary>
        IEntityDescriptorCollection EntityDescriptorCollection { get; }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        IDbConnection OpenConnection();

        /// <summary>
        /// 日志工厂
        /// </summary>
        ILoggerFactory LoggerFactory { get; }
    }
}
