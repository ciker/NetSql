using System.Data;
using System.Data.SqlClient;
using NetSql.Internal;
using NetSql.SqlAdapter;
using DatabaseType = NetSql.Enums.DatabaseType;

namespace NetSql
{
    /// <summary>
    /// 数据库上下文配置项SqlServer实现
    /// </summary>
    public class DbContextOptions : IDbContextOptions
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        public ISqlAdapter SqlAdapter { get; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection => new SqlConnection(ConnectionString);

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="openLogger"></param>
        public DbContextOptions(string connectionString, bool openLogger = false)
        {
            Check.NotNull(connectionString, nameof(connectionString), "数据库连接字符串为空");

            ConnectionString = connectionString;
            DbType = DatabaseType.SqlServer;
            SqlAdapter = new SqlServerAdapter();
        }
    }
}
