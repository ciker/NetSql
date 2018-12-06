using System.Text;
using NetSql.Core.SqlAdapter;

namespace NetSql.SqlServer
{
    public class SqlServerAdapter : SqlAdapterAbstract
    {
        /// <summary>
        /// 左引号
        /// </summary>
        public override char LeftQuote => '[';

        /// <summary>
        /// 右引号
        /// </summary>
        public override char RightQuote => ']';

        /// <summary>
        /// 获取最后新增ID语句
        /// </summary>
        public override string IdentitySql => "SELECT SCOPE_IDENTITY() ID;";

        public override string GeneratePagingSql(string @select, string table, string where, string sort, int skip, int take)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("SELECT * FROM (SELECT {0},ROW_NUMBER() OVER(ORDER BY {1}) AS row_num FROM {2}", select, sort, table);
            if (!string.IsNullOrWhiteSpace(where))
                sqlBuilder.AppendFormat(" WHERE {0}", where);

            sqlBuilder.AppendFormat(") AS m WHERE m.row_num BETWEEN {0} AND {1}", skip + 1, skip + take);
            return sqlBuilder.ToString();
        }
    }
}
