using System.Data;
using Microsoft.Data.Sqlite;
using NetSql.Enums;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.SQLite
{
    public class SQLiteDbContextOptions : IDbContextOptions
    {
        public string ConnectionString { get; }

        public ISqlAdapter SqlAdapter { get; }

        public IDbConnection DbConnection => new SqliteConnection(ConnectionString);

        public DatabaseType DbType { get; }

        public SQLiteDbContextOptions(string connectionString)
        {
            Check.NotNull(connectionString, nameof(connectionString), "数据库连接字符串为空");

            ConnectionString = connectionString;
            DbType = DatabaseType.MySql;
            SqlAdapter = new SQLiteAdapter();
        }
    }
}
