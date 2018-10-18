using NetSql.Enums;

namespace NetSql.Integration
{
    public class NetSqlOptions
    {
        public string Name { get; set; }

        public DbType DbType { get; set; }

        public string ConnString { get; set; }
    }
}
