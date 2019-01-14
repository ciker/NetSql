using System.Data;
using Microsoft.Data.Sqlite;
using NetSql.Core;

namespace NetSql.SQLite
{
    /// <summary>
    /// SQLite数据库上下文配置项
    /// </summary>
    public class SQLiteDbContextOptions : DbContextOptionsAbstract
    {
        public SQLiteDbContextOptions(string name, string connectionString) : base(name, connectionString, new SQLiteAdapter())
        {
        }

        public override IDbConnection OpenConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
