using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NetSql.Entities;
using NetSql.Enums;
using NetSql.Internal;
using NetSql.Pagination;
using NetSql.SqlAdapter;
using NetSql.SqlQueryable;

namespace NetSql.Expressions
{
    internal class ExpressionResolve : IExpressionResolve
    {
        private readonly ISqlAdapter _sqlAdapter;
        private readonly JoinCollection _joinCollection;

        private bool IsJoin => _joinCollection.Count > 1;


        public ExpressionResolve(ISqlAdapter sqlAdapter, JoinCollection joinCollection)
        {
            _sqlAdapter = sqlAdapter;
            _joinCollection = joinCollection;
        }

        public string ResolveWhere(Expression whereExpression)
        {
            var sqlBuilder = new StringBuilder();
            ResolveWhere(sqlBuilder, whereExpression);
            return sqlBuilder.ToString();
        }

        public void ResolveWhere(StringBuilder sqlBuilder, Expression whereExpression)
        {
            if (whereExpression == null)
                return;

            Resolve(whereExpression, sqlBuilder);

            //删除多余的括号
            if (sqlBuilder.Length > 1 && sqlBuilder[0] == '(' && sqlBuilder[sqlBuilder.Length - 1] == ')')
            {
                sqlBuilder.Remove(0, 1);
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            }
        }

        public string ResolveSelect(Expression selectExpression)
        {
            var sqlBuilder = new StringBuilder();
            ResolveSelect(sqlBuilder, selectExpression);
            return sqlBuilder.ToString();
        }

        public void ResolveSelect(StringBuilder sql, Expression selectExpression)
        {
            if (selectExpression is LambdaExpression lambda)
            {
                if (lambda.Body is ParameterExpression parameterExpression)
                {
                    ResolveSelect(sql, parameterExpression);
                }
                else if (lambda.Body is MemberExpression memberExpression)
                {
                    ResolveSelect(sql, memberExpression);
                }
                else if (lambda.Body is NewExpression newExpression)
                {
                    for (var i = 0; i < newExpression.Arguments.Count; i++)
                    {
                        var arg = newExpression.Arguments[i];
                        if (arg is MemberExpression memberExpression1)
                        {
                            AppendColum(sql, memberExpression1);
                            if (memberExpression1.Member.Name != newExpression.Members[i].Name)
                            {
                                sql.AppendFormat(" AS {0}", _sqlAdapter.AppendQuote(newExpression.Members[i].Name));
                            }
                            else
                            {
                                sql.AppendFormat(" AS {0}", _sqlAdapter.AppendQuote(memberExpression1.Member.Name));
                            }
                        }
                        else if (arg is ParameterExpression p1)
                        {
                            ResolveSelect(sql, p1);
                        }

                        if (i < newExpression.Arguments.Count - 1)
                            sql.Append(",");
                    }
                }
            }
            else
            {
                var descriptor = _joinCollection.First();
                for (var i = 0; i < descriptor.EntityDescriptor.Columns.Count; i++)
                {
                    var col = descriptor.EntityDescriptor.Columns[i];
                    AppendColum(sql, descriptor, col);
                    sql.AppendFormat(" AS {0}", _sqlAdapter.AppendQuote(col.PropertyInfo.Name));
                    if (i < descriptor.EntityDescriptor.Columns.Count - 1)
                        sql.Append(",");
                }
            }
        }

        public string ResolveJoin()
        {
            var sqlBuilder = new StringBuilder();
            ResolveJoin(sqlBuilder);
            return sqlBuilder.ToString();
        }

        public void ResolveJoin(StringBuilder sqlBuilder)
        {
            var first = _joinCollection.First();

            if (!IsJoin)
            {
                sqlBuilder.AppendFormat(" {0} ", _sqlAdapter.AppendQuote(first.EntityDescriptor.TableName));
                return;
            }
            sqlBuilder.AppendFormat(" {0} AS {1} ", _sqlAdapter.AppendQuote(first.EntityDescriptor.TableName), _sqlAdapter.AppendQuote(first.Alias));

            for (var i = 1; i < _joinCollection.Count; i++)
            {
                var descriptor = _joinCollection[i];
                switch (descriptor.Type)
                {
                    case JoinType.Inner:
                        sqlBuilder.Append(" INNER ");
                        break;
                    case JoinType.Right:
                        sqlBuilder.Append(" RIGHT ");
                        break;
                    default:
                        sqlBuilder.Append(" LEFT ");
                        break;
                }
                sqlBuilder.AppendFormat("JOIN {0} AS {1} ON ", _sqlAdapter.AppendQuote(descriptor.EntityDescriptor.TableName), _sqlAdapter.AppendQuote(descriptor.Alias));
                ResolveWhere(sqlBuilder, descriptor.On);
            }
        }

        public void ResolveOrder(StringBuilder sqlBuilder, List<Sort> sorts)
        {
            if (sorts.Any())
            {
                for (var i = 0; i < sorts.Count; i++)
                {
                    var sort = sorts[i];
                    sqlBuilder.AppendFormat(" {0} {1}", sort.OrderBy, sort.Type == SortType.Asc ? "ASC" : "DESC");

                    if (i < sorts.Count - 1)
                        sqlBuilder.Append(",");
                }
            }
        }

        public string ResolveOrder(List<Sort> sorts)
        {
            var sqlBuilder = new StringBuilder();
            ResolveOrder(sqlBuilder, sorts);
            return sqlBuilder.ToString();
        }

        private void ResolveSelect(StringBuilder sql, ParameterExpression exp)
        {
            var descriptor = _joinCollection.Get(exp.Type);
            for (var i = 0; i < descriptor.EntityDescriptor.Columns.Count; i++)
            {
                var col = descriptor.EntityDescriptor.Columns[i];
                AppendColum(sql, descriptor, col);
                sql.AppendFormat(" AS {0}", _sqlAdapter.AppendQuote(col.PropertyInfo.Name));
                if (i < descriptor.EntityDescriptor.Columns.Count - 1)
                    sql.Append(",");
            }
        }

        #region ==表达式解析==

        private void Resolve(Expression exp, StringBuilder sqlBuilder)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    LambdaResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    BinaryResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Constant:
                    ConstantResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.MemberAccess:
                    MemberAccessResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Call:
                    CallResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.Not:
                    NotResolve(exp, sqlBuilder);
                    break;
                case ExpressionType.MemberInit:
                    MemberInitResolve(exp, sqlBuilder);
                    break;
            }
        }

        private void LambdaResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is LambdaExpression lambdaExp))
                return;

            Resolve(lambdaExp.Body, sqlBuilder);
        }

        private void BinaryResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is BinaryExpression binaryExp))
                return;

            sqlBuilder.Append("(");

            Resolve(binaryExp.Left, sqlBuilder);

            switch (binaryExp.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sqlBuilder.Append(" AND ");
                    break;
                case ExpressionType.GreaterThan:
                    sqlBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sqlBuilder.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    sqlBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sqlBuilder.Append(" <= ");
                    break;
                case ExpressionType.Equal:
                    sqlBuilder.Append(" = ");
                    break;
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    sqlBuilder.Append(" OR ");
                    break;
                case ExpressionType.NotEqual:
                    sqlBuilder.Append("<>");
                    break;
            }

            Resolve(binaryExp.Right, sqlBuilder);

            sqlBuilder.Append(")");
        }

        private void ConstantResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is ConstantExpression constantExp))
                return;

            if (exp.Type == typeof(string))
                sqlBuilder.AppendFormat("'{0}'", constantExp.Value);
            else if (exp.Type == typeof(bool))
                sqlBuilder.AppendFormat("{0}", constantExp.Value.ToBool().ToIntString());
            else if (exp.Type.IsEnum)
                sqlBuilder.AppendFormat("{0}", constantExp.Value.ToInt());
            else
                sqlBuilder.Append(constantExp.Value);
        }

        private void MemberAccessResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is MemberExpression memberExp))
                return;

            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                AppendColum(sqlBuilder, memberExp);
            }
            else
            {
                //对于非实体属性的成员，如外部变量等
                DynamicInvokeResolve(exp, sqlBuilder);
            }
        }

        private void UnaryResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is UnaryExpression unaryExp))
                return;

            Resolve(unaryExp.Operand, sqlBuilder);
        }

        private void DynamicInvokeResolve(Expression exp, StringBuilder sqlBuilder)
        {
            var value = DynamicInvoke(exp);

            if (exp.Type == typeof(DateTime) || exp.Type == typeof(string) || exp.Type == typeof(char))
                sqlBuilder.AppendFormat("'{0}'", value);
            else if (exp.Type.IsEnum)
                sqlBuilder.AppendFormat("{0}", value.ToInt());
            else
                sqlBuilder.AppendFormat("{0}", value);
        }

        private void CallResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is MethodCallExpression callExp))
                return;

            var methodName = callExp.Method.Name;
            if (methodName.Equals("StartsWith"))
            {
                StartsWithResolve(callExp, sqlBuilder);
            }
            else if (methodName.Equals("EndsWith"))
            {
                EndsWithResolve(callExp, sqlBuilder);
            }
            else if (methodName.Equals("Contains"))
            {
                ContainsResolve(callExp, sqlBuilder);
            }
            else if (methodName.Equals("Equals"))
            {
                EqualsResolve(callExp, sqlBuilder);
            }
            else
            {
                DynamicInvokeResolve(exp, sqlBuilder);
            }
        }

        private void StartsWithResolve(MethodCallExpression exp, StringBuilder sqlBuilder)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                AppendColum(sqlBuilder, objExp);

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                sqlBuilder.AppendFormat(" LIKE '{0}%'", value);
            }
        }

        private void EndsWithResolve(MethodCallExpression exp, StringBuilder sqlBuilder)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                AppendColum(sqlBuilder, objExp);

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                sqlBuilder.AppendFormat(" LIKE '%{0}'", value);
            }
        }

        private void ContainsResolve(MethodCallExpression exp, StringBuilder sqlBuilder)
        {
            if (exp.Object is MemberExpression objExp)
            {
                if (objExp.Expression.NodeType == ExpressionType.Parameter)
                {
                    AppendColum(sqlBuilder, objExp);

                    string value;
                    if (exp.Arguments[0] is ConstantExpression c)
                    {
                        value = c.Value.ToString();
                    }
                    else
                    {
                        value = DynamicInvoke(exp.Arguments[0]).ToString();
                    }

                    sqlBuilder.AppendFormat(" LIKE '%{0}%'", value);
                }
                else if (objExp.Type.IsGenericType && exp.Arguments[0] is MemberExpression argExp && argExp.Expression.NodeType == ExpressionType.Parameter)
                {
                    AppendColum(sqlBuilder, argExp);

                    sqlBuilder.Append(" IN (");

                    #region ==解析集合==

                    var constant = objExp.Expression as ConstantExpression;
                    var value = ((FieldInfo)objExp.Member).GetValue(constant.Value);
                    var valueType = objExp.Type.GenericTypeArguments[0];
                    var isValueType = false;
                    var list = new List<string>();
                    if (valueType == typeof(string))
                    {
                        list = value as List<string>;
                    }
                    else if (valueType == typeof(char))
                    {
                        if (value is List<char> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(DateTime))
                    {
                        if (value is List<DateTime> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                    }
                    else if (valueType == typeof(int))
                    {
                        isValueType = true;
                        if (value is List<int> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(long))
                    {
                        isValueType = true;
                        if (value is List<long> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(double))
                    {
                        isValueType = true;
                        if (value is List<double> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(float))
                    {
                        isValueType = true;
                        if (value is List<float> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }
                    else if (valueType == typeof(decimal))
                    {
                        isValueType = true;
                        if (value is List<decimal> valueList)
                        {
                            foreach (var c in valueList)
                            {
                                list.Add(c.ToString());
                            }
                        }
                    }

                    if (list == null)
                        return;

                    #endregion

                    //值类型不带引号
                    if (isValueType)
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            sqlBuilder.AppendFormat("{0}", list[i]);
                            if (i != list.Count - 1)
                            {
                                sqlBuilder.Append(",");
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            sqlBuilder.AppendFormat("'{0}'", list[i].Replace("'", "''"));
                            if (i != list.Count - 1)
                            {
                                sqlBuilder.Append(",");
                            }
                        }
                    }
                    sqlBuilder.Append(") ");
                }
            }
            else if (exp.Arguments[0].Type.IsArray && exp.Arguments[1] is MemberExpression argExp && argExp.Expression.NodeType == ExpressionType.Parameter)
            {
                AppendColum(sqlBuilder, argExp);
                sqlBuilder.Append(" IN (");

                #region ==解析数组==

                var member = exp.Arguments[0] as MemberExpression;
                var constant = member.Expression as ConstantExpression;
                var value = ((FieldInfo)member.Member).GetValue(constant.Value);
                var valueType = member.Type.FullName;
                var isValueType = false;
                string[] list = null;
                if (valueType == "System.String[]")
                {
                    list = value as string[];
                }
                else if (valueType == "System.Char[]")
                {
                    if (value is char[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Datetime[]")
                {
                    if (value is DateTime[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
                else if (valueType == "System.Int32[]")
                {
                    isValueType = true;
                    if (value is int[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Int64[]")
                {
                    isValueType = true;
                    if (value is long[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Double[]")
                {
                    isValueType = true;
                    if (value is double[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Single[]")
                {
                    isValueType = true;
                    if (value is float[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }
                else if (valueType == "System.Decimal[]")
                {
                    isValueType = true;
                    if (value is decimal[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
                }

                if (list == null)
                    return;

                #endregion

                //值类型不带引号
                if (isValueType)
                {
                    for (var i = 0; i < list.Length; i++)
                    {
                        sqlBuilder.AppendFormat("{0}", list[i]);
                        if (i != list.Length - 1)
                        {
                            sqlBuilder.Append(",");
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < list.Length; i++)
                    {
                        sqlBuilder.AppendFormat("'{0}'", list[i].Replace("'", "''"));
                        if (i != list.Length - 1)
                        {
                            sqlBuilder.Append(",");
                        }
                    }
                }
                sqlBuilder.Append(") ");
            }
        }

        private void EqualsResolve(MethodCallExpression exp, StringBuilder sqlBuilder)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                AppendColum(sqlBuilder, objExp);

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                sqlBuilder.AppendFormat(" = '{0}'", value);
            }
        }

        private void NotResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null)
                return;

            sqlBuilder.Append("(");

            UnaryResolve(exp, sqlBuilder);

            sqlBuilder.Append(" = 0)");
        }

        private void MemberInitResolve(Expression exp, StringBuilder sqlBuilder)
        {
            if (exp == null || !(exp is MemberInitExpression initExp) || !initExp.Bindings.Any())
                return;

            for (var i = 0; i < initExp.Bindings.Count; i++)
            {
                if (initExp.Bindings[i] is MemberAssignment assignment)
                {
                    var descriptor = _joinCollection.Get(initExp.Type);
                    var col = descriptor.EntityDescriptor.GetColumnByPropertyName(assignment.Member.Name);
                    if (col != null)
                    {
                        AppendColum(sqlBuilder, descriptor, col);
                        sqlBuilder.Append("=");
                        Resolve(assignment.Expression, sqlBuilder);

                        if (i < initExp.Bindings.Count - 1)
                            sqlBuilder.Append(",");
                    }
                }
            }
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private object DynamicInvoke(Expression exp)
        {
            return Expression.Lambda(exp).Compile().DynamicInvoke();
        }

        /// <summary>
        /// 附加列
        /// </summary>
        /// <param name="sqlBuilder"></param>
        /// <param name="kv"></param>
        /// <param name="col"></param>
        private void AppendFormatColumn(StringBuilder sqlBuilder, KeyValuePair<string, IEntityDescriptor> kv, ColumnDescriptor col)
        {
            if (IsJoin)
                sqlBuilder.AppendFormat("{0}.{1}", _sqlAdapter.AppendQuote(kv.Key), _sqlAdapter.AppendQuote(col.Name));
            else
                sqlBuilder.Append(_sqlAdapter.AppendQuote(col.Name));
        }

        #endregion

        #region ==附加列==

        private void AppendColum(StringBuilder sqlBuilder, MemberExpression exp)
        {
            sqlBuilder.Append(_joinCollection.GetColumn(exp));
        }

        private void AppendColum(StringBuilder sqlBuilder, JoinDescriptor descriptor, ColumnDescriptor col)
        {
            if (IsJoin)
                sqlBuilder.Append($"{_sqlAdapter.AppendQuote(descriptor.Alias)}.{_sqlAdapter.AppendQuote(col.Name)}");
            else
                sqlBuilder.Append($"{_sqlAdapter.AppendQuote(col.Name)}");
        }

        #endregion
    }
}
