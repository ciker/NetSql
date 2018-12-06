using System.Data;
using MySql.Data.MySqlClient;
using Td.Fw.Data.Core;
using Td.Fw.Data.Core.Enums;

namespace Td.Fw.Data.MySql
{
    /// <summary>
    /// MySql数据库上下文配置项
    /// </summary>
    public class MySqlDbContextOptions : DbContextOptionsAbstract
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public override IDbConnection DbConnection => new MySqlConnection(ConnectionString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public MySqlDbContextOptions(string connectionString) : base(connectionString, new MySqlAdapter(), DatabaseType.MySql)
        {

        }
    }
}
