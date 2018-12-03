using System.Data;
using System.Data.SqlClient;
using Td.Fw.Data.Core;
using Td.Fw.Data.Core.Enums;

namespace Td.Fw.Data.SqlServer
{
    /// <summary>
    /// 数据库上下文配置项SqlServer实现
    /// </summary>
    public class SqlServerDbContextOptions : DbContextOptionsAbstract
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public override IDbConnection DbConnection => new SqlConnection(ConnectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlServerDbContextOptions(string connectionString) : base(connectionString, new SqlServerAdapter(), DatabaseType.SqlServer)
        {

        }
    }
}