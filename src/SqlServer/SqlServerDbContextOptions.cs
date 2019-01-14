using System.Data;
using System.Data.SqlClient;
using NetSql.Core;

namespace NetSql.SqlServer
{
    /// <summary>
    /// 数据库上下文配置项SqlServer实现
    /// </summary>
    public class SqlServerDbContextOptions : DbContextOptionsAbstract
    {
        public SqlServerDbContextOptions(string name, string connectionString) : base(name, connectionString, new SqlServerAdapter())
        {
        }

        public override IDbConnection OpenConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}