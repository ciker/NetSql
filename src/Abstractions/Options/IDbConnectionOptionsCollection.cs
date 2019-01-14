using System.Collections.Generic;

namespace NetSql.Abstractions.Options
{
    /// <summary>
    /// 数据库连接配置项集合
    /// </summary>
    public interface IDbConnectionOptionsCollection : IList<DbConnectionOptions>
    {
      
    }
}
