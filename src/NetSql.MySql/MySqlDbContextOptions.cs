using System.Data;
using MySql.Data.MySqlClient;
using NetSql.Enums;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.MySql
{
    /// <summary>
    /// MySql数据库上下文配置项
    /// </summary>
    public class MySqlDbContextOptions : IDbContextOptions
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        public ISqlAdapter SqlAdapter { get; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection => new MySqlConnection(ConnectionString);

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public MySqlDbContextOptions(string connectionString)
        {
            Check.NotNull(connectionString, nameof(connectionString), "数据库连接字符串为空");

            ConnectionString = connectionString;
            DbType = DatabaseType.MySql;
            SqlAdapter = new MySqlAdapter();
        }
    }
}
