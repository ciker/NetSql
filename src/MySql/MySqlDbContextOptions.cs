using System.Data;
using MySql.Data.MySqlClient;
using NetSql.Core;

namespace NetSql.MySql
{
    /// <summary>
    /// MySql数据库上下文配置项
    /// </summary>
    public class MySqlDbContextOptions : DbContextOptionsAbstract
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">连接名称</param>
        /// <param name="connectionString">连接字符串</param>
        public MySqlDbContextOptions(string name, string connectionString) : base(name, connectionString, new MySqlAdapter())
        {
        }

        public override IDbConnection OpenConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
