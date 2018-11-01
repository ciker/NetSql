using NetSql.Enums;

namespace NetSql.Integration
{
    public class NetSqlOptions
    {
        public string Name { get; set; }

        public DatabaseType DbType { get; set; }

        public string ConnString { get; set; }
    }
}
