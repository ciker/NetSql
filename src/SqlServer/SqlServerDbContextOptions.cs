using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NetSql.Core;

namespace NetSql.SqlServer
{
    /// <summary>
    /// 数据库上下文配置项SqlServer实现
    /// </summary>
    public class SqlServerDbContextOptions : DbContextOptionsAbstract
    {
        public SqlServerDbContextOptions(string name, string connectionString, ILoggerFactory loggerFactory = null) : base(name, connectionString, new SqlServerAdapter(), loggerFactory)
        {
        }

        public override IDbConnection OpenConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}