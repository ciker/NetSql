using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NetSql.Internal;

namespace NetSql.Integration
{
    public class NetSqlOptionsContext
    {
        private const string Key = "DbConnection";

        public List<NetSqlOptions> OptionsList { get; }

        public NetSqlOptionsContext(IConfiguration cfg)
        {
            Check.NotNull(cfg, "IConfiguration");

            OptionsList = new List<NetSqlOptions>();
            cfg.GetSection(Key).Bind(OptionsList);

            foreach (var options in OptionsList)
            {
                Check.NotNull(options.ConnString, "DbConnection", $"{options.Name}数据库连接字符串为空");
            }
        }
    }
}
