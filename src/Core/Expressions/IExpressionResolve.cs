using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Td.Fw.Data.Core.Pagination;

namespace Td.Fw.Data.Core.Expressions
{
    internal interface IExpressionResolve
    {
        /// <summary>
        /// 解析条件
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        string ResolveWhere(Expression whereExpression);

        /// <summary>
        /// 解析条件
        /// </summary>
        /// <param name="sqlBuilder"></param>
        /// <param name="whereExpression"></param>
        void ResolveWhere(StringBuilder sqlBuilder, Expression whereExpression);

        /// <summary>
        /// 解析返回列
        /// </summary>
        /// <param name="selectExpression"></param>
        string ResolveSelect(Expression selectExpression);

        /// <summary>
        /// 解析返回列
        /// </summary>
        /// <param name="sqlBuilder"></param>
        /// <param name="selectExpression"></param>
        void ResolveSelect(StringBuilder sqlBuilder, Expression selectExpression);

        /// <summary>
        /// 解析表连接
        /// </summary>
        string ResolveJoin();

        /// <summary>
        /// 解析表连接
        /// </summary>
        /// <param name="sqlBuilder"></param>
        void ResolveJoin(StringBuilder sqlBuilder);

        /// <summary>
        /// 解析排序
        /// </summary>
        /// <param name="sorts"></param>
        /// <returns></returns>
        string ResolveOrder(List<Sort> sorts);

        /// <summary>
        /// 解析排序
        /// </summary>
        /// <param name="sqlBuilder"></param>
        /// <param name="sorts"></param>
        void ResolveOrder(StringBuilder sqlBuilder, List<Sort> sorts);
    }
}
