using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Logging;
using NetSql.Core;

namespace NetSql.SQLite
{
    /// <summary>
    /// SQLite数据库上下文配置项
    /// </summary>
    public class SQLiteDbContextOptions : DbContextOptionsAbstract
    {
        public SQLiteDbContextOptions(string name, string connectionString, ILoggerFactory loggerFactory = null) : base(name, connectionString, new SQLiteAdapter(), loggerFactory)
        {
        }

        public override IDbConnection OpenConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
