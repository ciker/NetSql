using System.Text;
using NetSql.Internal;
using NetSql.SqlAdapter;

namespace NetSql.MySql
{
    internal class MySqlAdapter : SqlAdapterAbstract
    {
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
            if (where.NotNull())
                sqlBuilder.AppendFormat(" WHERE {0}", where);

            if (sort.NotNull())
                sqlBuilder.AppendFormat("ORDER BY {0}", sort);

            sqlBuilder.AppendFormat(" LIMIT {0},{1}", skip, take);
            return sqlBuilder.ToString();
        }
    }
}
