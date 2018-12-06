using System.Data;
using Td.Fw.Data.Core.Enums;
using Td.Fw.Data.Core.Internal;
using Td.Fw.Data.Core.SqlAdapter;

namespace Td.Fw.Data.Core
{
    /// <summary>
    /// 数据库上下文抽象类
    /// </summary>
    public abstract class DbContextOptionsAbstract : IDbContextOptions
    {
        public string ConnectionString { get; }
        public ISqlAdapter SqlAdapter { get; }
        public abstract IDbConnection DbConnection { get; }
        public DatabaseType DbType { get; }

        public DbContextOptionsAbstract(string connectionString, ISqlAdapter sqlAdapter, DatabaseType dbType)
        {
            Check.NotNull(connectionString, nameof(connectionString), "数据库连接字符串为空");
            ConnectionString = connectionString;
            SqlAdapter = sqlAdapter;
            DbType = dbType;
        }
    }
}
