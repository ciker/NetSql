using System.Data;
using System.Data.SQLite;
using Td.Fw.Data.Core;
using Td.Fw.Data.Core.Enums;

namespace Td.Fw.Data.SQLite
{
    /// <summary>
    /// SQLite数据库上下文配置项
    /// </summary>
    public class SQLiteDbContextOptions : DbContextOptionsAbstract
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public override IDbConnection DbConnection => new SQLiteConnection(ConnectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLiteDbContextOptions(string connectionString) : base(connectionString, new SQLiteAdapter(), DatabaseType.SQLite)
        {

        }
    }
}
