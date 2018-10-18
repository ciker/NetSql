using System.Collections.Generic;
using System.Linq.Expressions;
using Oldli.Fw.Utils.Pagination;

namespace NetSql.SqlQueryable
{
    internal class QueryableBody
    {
        public List<Sort> Sorts { get; } = new List<Sort>();

        public LambdaExpression Select { get; set; }

        public LambdaExpression Where { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}
