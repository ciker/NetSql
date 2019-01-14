using System.Collections.Generic;
using System.Linq.Expressions;
using NetSql.Abstractions.Pagination;

namespace NetSql.Core.SqlQueryable
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
