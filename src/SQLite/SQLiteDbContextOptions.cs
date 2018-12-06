#if NETSTANDARD2_0
using Microsoft.Data.Sqlite;
#else
using System.Data.SQLite;
#endif
using System.Data;
using Td.Fw.Data.Core;
using Td.Fw.Data.Core.Enums;

namespace Td.Fw.Data.SQLite
{
    /// <summary>
    /// SQLite数据库上下文配置项
    /// </summary>
    public class SQLiteDbContextOptions : DbContextOptionsAbstract
    {

#if NETSTANDARD2_0
        /// <summary>
        /// 数据库连接
        /// </summary>
        public override IDbConnection DbConnection => new SqliteConnection(ConnectionString);
#else
        /// <summary>
        /// 数据库连接
        /// </summary>
        public override IDbConnection DbConnection => new SQLiteConnection(ConnectionString);
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLiteDbContextOptions(string connectionString) : base(connectionString, new SQLiteAdapter(), DatabaseType.SQLite)
        {

        }
    }
}
