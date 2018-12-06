using System.Text;
using NetSql.Core.Enums;
using NetSql.Core.SqlAdapter;

namespace NetSql.MySql
{
    internal class MySqlAdapter : SqlAdapterAbstract
    {
        public override DatabaseType Type => DatabaseType.MySql;

        /// <summary>
        /// 左引号
        /// </summary>
        public override char LeftQuote => '`';

        /// <summary>
        /// 右引号
        /// </summary>
        public override char RightQuote => '`';

        /// <summary>
        /// 获取最后新增ID语句
        /// </summary>
        public override string IdentitySql => "SELECT LAST_INSERT_ID() ID;";

        public override string GeneratePagingSql(string @select, string table, string @where, string sort, int skip, int take)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("SELECT {0} FROM {1}", select, table);
            if (!string.IsNullOrWhiteSpace(where))
                sqlBuilder.AppendFormat(" WHERE {0}", where);

            if (!string.IsNullOrWhiteSpace(sort))
                sqlBuilder.AppendFormat("ORDER BY {0}", sort);

            sqlBuilder.AppendFormat(" LIMIT {0},{1}", skip, take);
            return sqlBuilder.ToString();
        }
    }
}
