using NetSql.Abstractions.Enums;

namespace NetSql.Abstractions.Options
{
    public class DbConnectionOptions
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public SqlDialect Dialect { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; set; }
    }
}
