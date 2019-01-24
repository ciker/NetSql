using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NetSql.Abstractions;
using NetSql.Abstractions.Enums;
using NetSql.Core.Internal;

namespace NetSql.Core.SqlQueryable.Internal
{
   internal class QueryBuilder
    {
        #region ==字段==

        private readonly QueryBody _queryBody;
        private readonly ISqlAdapter _sqlAdapter;
        private readonly ILogger _logger;

        #endregion

        #region ==构造函数==

        public QueryBuilder(QueryBody queryBody, ISqlAdapter sqlAdapter, ILogger logger)
        {
            _queryBody = queryBody;
            _sqlAdapter = sqlAdapter;
            _logger = logger;
        }

        #endregion

        #region ==方法==

        public string CountSqlBuild(out QueryParameters parameters)
        {
            parameters = new QueryParameters();

            var sqlBuilder = new StringBuilder("SELECT COUNT(*) FROM ");

            ResolveFrom(sqlBuilder, parameters);

            ResolveWhere(sqlBuilder, parameters);

            var sql = sqlBuilder.ToString();

            _logger?.LogDebug("Count:" + sql);

            return sql;
        }

        public string UpdateSqlBuild(string tableName, out QueryParameters parameters)
        {
            Check.NotNull(tableName, nameof(tableName), "未指定更新表");

            var sqlBuilder = new StringBuilder();
            parameters = new QueryParameters();

            var updateSql = ResolveUpdate(parameters);
            Check.NotNull(updateSql, nameof(updateSql), "生成更新sql异常");

            sqlBuilder.AppendFormat("UPDATE {0} SET ", tableName);
            sqlBuilder.Append(updateSql);

            var whereSql = ResolveWhere(parameters);
            Check.NotNull(whereSql, nameof(whereSql), "生成条件sql异常");
            sqlBuilder.AppendFormat(" WHERE {0}", whereSql);

            var sql = sqlBuilder.ToString();

            _logger?.LogDebug("Update:" + sql);

            return sql;
        }

        public string DeleteSqlBuild(string tableName, out QueryParameters parameters)
        {
            Check.NotNull(tableName, nameof(tableName), "未指定更新表");

            var sqlBuilder = new StringBuilder();
            parameters = new QueryParameters();

            sqlBuilder.AppendFormat("DELETE FROM {0} ", tableName);

            var whereSql = ResolveWhere(parameters);
            Check.NotNull(whereSql, nameof(whereSql), "生成条件sql异常");
            sqlBuilder.AppendFormat(" WHERE {0}", whereSql);

            var sql = sqlBuilder.ToString();

            _logger?.LogDebug("Delete:" + sql);

            return sql;
        }

        public string MaxSqlBuild(out QueryParameters parameters)
        {
            Check.NotNull(_queryBody.Max, nameof(_queryBody.Max), "未指定求最大值的列");

            var memberExpression = _queryBody.Max.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("无法解析表达式", nameof(_queryBody.Max));

            var sqlBuilder = new StringBuilder("SELECT ");
            parameters = new QueryParameters();
            sqlBuilder.AppendFormat("MAX({0}) FROM ", _queryBody.GetColumnName(memberExpression));

            ResolveFrom(sqlBuilder, parameters);

            ResolveWhere(sqlBuilder, parameters);

            var sql = sqlBuilder.ToString();

            _logger?.LogDebug("Max:" + sql);

            return sql;
        }

        public string MinSqlBuild(out QueryParameters parameters)
        {
            Check.NotNull(_queryBody.Max, nameof(_queryBody.Max), "未指定求最小值的列");

            var memberExpression = _queryBody.Max.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("无法解析表达式", nameof(_queryBody.Max));

            var sqlBuilder = new StringBuilder("SELECT ");
            parameters = new QueryParameters();
            sqlBuilder.AppendFormat("MIN({0}) FROM ", _queryBody.GetColumnName(memberExpression));

            ResolveFrom(sqlBuilder, parameters);

            ResolveWhere(sqlBuilder, parameters);

            var sql = sqlBuilder.ToString();

            _logger?.LogDebug("Min:" + sql);

            return sql;
        }

        public string FirstSqlBuild(out QueryParameters parameters)
        {
            parameters = new QueryParameters();
            var select = ResolveSelect();
            var from = ResolveFrom(parameters);
            var where = ResolveWhere(parameters);
            var sort = ResolveOrder();

            var sql = _sqlAdapter.GenerateFirstSql(select, from, where, sort);

            _logger?.LogDebug("First:" + sql);

            return sql;
        }

        public string ExistsSqlBuild(out QueryParameters parameters)
        {
            parameters = new QueryParameters();

            var select = _sqlAdapter.SqlDialect == SqlDialect.SqlServer ? "" : "1";
            var from = ResolveFrom(parameters);
            var where = ResolveWhere(parameters);
            var sort = ResolveOrder();

            var sql = _sqlAdapter.GenerateFirstSql(select, from, where, sort);

            _logger?.LogDebug("Exists:{0}", sql);

            return sql;
        }

        public string QuerySqlBuild(out QueryParameters parameters)
        {
            string sql;
            parameters = new QueryParameters();

            //分页查询
            if (_queryBody.Take > 0)
            {
                var select = ResolveSelect();
                var from = ResolveFrom(parameters);
                var where = ResolveWhere(parameters);
                var sort = ResolveOrder();

                #region ==SqlServer分页需要指定排序==

                //SqlServer分页需要指定排序，此处判断是否有主键，有主键默认按照主键排序
                if (_sqlAdapter.SqlDialect == SqlDialect.SqlServer && sort.IsNull())
                {
                    var first = _queryBody.JoinDescriptors.First();
                    if (first.EntityDescriptor.PrimaryKey.IsNo())
                    {
                        throw new ArgumentNullException("OrderBy", "SqlServer数据库没有主键的表需要指定排序字段才可以分页查询");
                    }

                    if (_queryBody.JoinDescriptors.Count > 1)
                    {
                        sort = $"{_sqlAdapter.AppendQuote(first.Alias)}.{_sqlAdapter.AppendQuote(first.EntityDescriptor.PrimaryKey.Name)}";
                    }
                    else
                    {
                        sort = first.EntityDescriptor.PrimaryKey.Name;
                    }
                }

                #endregion

                sql = _sqlAdapter.GeneratePagingSql(select, from, where, sort, _queryBody.Skip, _queryBody.Take);
            }
            else
            {
                var sqlBuilder = new StringBuilder("SELECT ");

                ResolveSelect(sqlBuilder);

                sqlBuilder.Append(" FROM ");

                ResolveFrom(sqlBuilder, parameters);

                ResolveWhere(sqlBuilder, parameters);

                ResolveOrder(sqlBuilder);

                sql = sqlBuilder.ToString();
            }

            _logger?.LogDebug("Query:{0}", sql);

            return sql;
        }

        #endregion

        #region ==解析Body==

        private string ResolveFrom(QueryParameters parameters)
        {
            var sqlBuilder = new StringBuilder();
            ResolveFrom(sqlBuilder, parameters);
            return sqlBuilder.ToString();
        }

        private void ResolveFrom(StringBuilder sqlBuilder, QueryParameters parameters)
        {
            var first = _queryBody.JoinDescriptors.First();

            sqlBuilder.AppendFormat(" {0} AS {1} ", _sqlAdapter.AppendQuote(first.EntityDescriptor.TableName), _sqlAdapter.AppendQuote(first.Alias));

            for (var i = 1; i < _queryBody.JoinDescriptors.Count; i++)
            {
                var descriptor = _queryBody.JoinDescriptors[i];
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
                sqlBuilder.Append(Resolve(descriptor.On, parameters));
            }
        }

        private string ResolveWhere(QueryParameters parameters)
        {
            return Resolve(_queryBody.Where, parameters);
        }

        private void ResolveWhere(StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (_queryBody.Where == null)
                return;

            var whereSql = Resolve(_queryBody.Where, parameters);
            if (whereSql.NotNull())
            {
                sqlBuilder.AppendFormat(" WHERE {0}", whereSql);
            }
        }

        private string ResolveUpdate(QueryParameters parameters)
        {
            Check.NotNull(_queryBody.Update, nameof(_queryBody.Update), "未指定更新字段");

            var sql = Resolve(_queryBody.Update, parameters);

            Check.NotNull(sql, nameof(_queryBody.Update), "更新表达式解析失败");

            return sql;
        }

        private string ResolveOrder()
        {
            var sqlBuilder = new StringBuilder();
            if (_queryBody.Sorts.Any())
            {
                _queryBody.Sorts.ForEach(sort =>
                {
                    sqlBuilder.AppendFormat(" {0} {1},", sort.OrderBy, sort.Type == SortType.Asc ? "ASC" : "DESC");
                });

                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            }

            return sqlBuilder.ToString();
        }

        private void ResolveOrder(StringBuilder sqlBuilder)
        {
            var sql = ResolveOrder();
            if (sql.NotNull())
                sqlBuilder.AppendFormat(" ORDER BY {0}", sql);
        }

        private string ResolveSelect()
        {
            var sqlBuilder = new StringBuilder();

            ResolveSelect(sqlBuilder, _queryBody.Select);

            return sqlBuilder.ToString();
        }

        private void ResolveSelect(StringBuilder sqlBuilder)
        {
            ResolveSelect(sqlBuilder, _queryBody.Select);
        }

        private void ResolveSelect(StringBuilder sqlBuilder, Expression selectExpression)
        {
            if (selectExpression is LambdaExpression lambda)
            {
                if (lambda.Body is ParameterExpression parameterExpression)
                {
                    ResolveSelect(sqlBuilder, parameterExpression);
                }
                else if (lambda.Body is MemberExpression memberExpression)
                {
                    ResolveSelect(sqlBuilder, memberExpression);
                }
                else if (lambda.Body is NewExpression newExpression)
                {
                    for (var i = 0; i < newExpression.Arguments.Count; i++)
                    {
                        var arg = newExpression.Arguments[i];
                        if (arg is MemberExpression memberExpression1)
                        {
                            sqlBuilder.Append(_queryBody.GetColumnName(memberExpression1));
                            if (memberExpression1.Member.Name != newExpression.Members[i].Name)
                            {
                                sqlBuilder.AppendFormat(" AS {0}", _sqlAdapter.AppendQuote(newExpression.Members[i].Name));
                            }
                            else
                            {
                                sqlBuilder.AppendFormat(" AS {0}", _sqlAdapter.AppendQuote(memberExpression1.Member.Name));
                            }
                        }
                        else if (arg is ParameterExpression p1)
                        {
                            ResolveSelect(sqlBuilder, p1);
                        }

                        if (i < newExpression.Arguments.Count - 1)
                            sqlBuilder.Append(",");
                    }
                }
            }
            else
            {
                var descriptor = _queryBody.JoinDescriptors.First();
                for (var i = 0; i < descriptor.EntityDescriptor.Columns.Count; i++)
                {
                    var col = descriptor.EntityDescriptor.Columns[i];
                    sqlBuilder.Append($"{_sqlAdapter.AppendQuote(descriptor.Alias)}.{_sqlAdapter.AppendQuote(col.Name)}");
                    sqlBuilder.AppendFormat(" AS {0}", _sqlAdapter.AppendQuote(col.PropertyInfo.Name));
                    if (i < descriptor.EntityDescriptor.Columns.Count - 1)
                        sqlBuilder.Append(",");
                }
            }
        }

        #endregion

        #region ==表达式解析==

        private string Resolve(Expression whereExpression, QueryParameters parameters)
        {
            if (whereExpression == null)
                return string.Empty;

            var sqlBuilder = new StringBuilder();

            Resolve(whereExpression, sqlBuilder, parameters);

            //删除多余的括号
            if (sqlBuilder.Length > 1 && sqlBuilder[0] == '(' && sqlBuilder[sqlBuilder.Length - 1] == ')')
            {
                sqlBuilder.Remove(0, 1);
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            }

            return sqlBuilder.ToString();
        }

        private void Resolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    LambdaResolve(exp, sqlBuilder, parameters);
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
                    BinaryResolve(exp, sqlBuilder, parameters);
                    break;
                case ExpressionType.Constant:
                    ConstantResolve(exp, sqlBuilder, parameters);
                    break;
                case ExpressionType.MemberAccess:
                    MemberAccessResolve(exp, sqlBuilder, parameters);
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryResolve(exp, sqlBuilder, parameters);
                    break;
                case ExpressionType.Call:
                    CallResolve(exp, sqlBuilder, parameters);
                    break;
                case ExpressionType.Not:
                    NotResolve(exp, sqlBuilder, parameters);
                    break;
                case ExpressionType.MemberInit:
                    MemberInitResolve(exp, sqlBuilder, parameters);
                    break;
            }
        }

        private void LambdaResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null || !(exp is LambdaExpression lambdaExp))
                return;

            Resolve(lambdaExp.Body, sqlBuilder, parameters);
        }

        private void BinaryResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null || !(exp is BinaryExpression binaryExp))
                return;

            sqlBuilder.Append("(");

            Resolve(binaryExp.Left, sqlBuilder, parameters);

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

            Resolve(binaryExp.Right, sqlBuilder, parameters);

            sqlBuilder.Append(")");
        }

        private void ConstantResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null || !(exp is ConstantExpression constantExp))
                return;

            AppendValue(sqlBuilder, parameters, constantExp.Value);
        }

        private void MemberAccessResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null || !(exp is MemberExpression memberExp))
                return;

            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                sqlBuilder.Append(_queryBody.GetColumnName(memberExp));
            }
            else
            {
                //对于非实体属性的成员，如外部变量等
                DynamicInvokeResolve(exp, sqlBuilder, parameters);
            }
        }

        private void UnaryResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null || !(exp is UnaryExpression unaryExp))
                return;

            Resolve(unaryExp.Operand, sqlBuilder, parameters);
        }

        private void DynamicInvokeResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            var value = DynamicInvoke(exp);

            AppendValue(sqlBuilder, parameters, value);
        }

        private void CallResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null || !(exp is MethodCallExpression callExp))
                return;

            var methodName = callExp.Method.Name;
            if (methodName.Equals("StartsWith"))
            {
                StartsWithResolve(callExp, sqlBuilder, parameters);
            }
            else if (methodName.Equals("EndsWith"))
            {
                EndsWithResolve(callExp, sqlBuilder, parameters);
            }
            else if (methodName.Equals("Contains"))
            {
                ContainsResolve(callExp, sqlBuilder, parameters);
            }
            else if (methodName.Equals("Equals"))
            {
                EqualsResolve(callExp, sqlBuilder, parameters);
            }
            else
            {
                DynamicInvokeResolve(exp, sqlBuilder, parameters);
            }
        }

        private void StartsWithResolve(MethodCallExpression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                sqlBuilder.Append(_queryBody.GetColumnName(objExp));

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                sqlBuilder.Append(" LIKE ");

                AppendValue(sqlBuilder, parameters, $"{value}%");
            }
        }

        private void EndsWithResolve(MethodCallExpression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                sqlBuilder.Append(_queryBody.GetColumnName(objExp));

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                sqlBuilder.Append(" LIKE ");

                AppendValue(sqlBuilder, parameters, $"%{value}");
            }
        }

        private void ContainsResolve(MethodCallExpression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp.Object is MemberExpression objExp)
            {
                if (objExp.Expression.NodeType == ExpressionType.Parameter)
                {
                    sqlBuilder.Append(_queryBody.GetColumnName(objExp));

                    string value;
                    if (exp.Arguments[0] is ConstantExpression c)
                    {
                        value = c.Value.ToString();
                    }
                    else
                    {
                        value = DynamicInvoke(exp.Arguments[0]).ToString();
                    }

                    sqlBuilder.Append(" LIKE ");

                    AppendValue(sqlBuilder, parameters, $"%{value}%");
                }
                else if (objExp.Type.IsGenericType && exp.Arguments[0] is MemberExpression argExp && argExp.Expression.NodeType == ExpressionType.Parameter)
                {
                    sqlBuilder.Append(_queryBody.GetColumnName(argExp));

                    sqlBuilder.Append(" IN (");

                    #region ==解析集合==

                    if (objExp.Expression is ConstantExpression constant)
                    {
                        var value = ((FieldInfo)objExp.Member).GetValue(constant.Value);
                        var valueType = objExp.Type.GetGenericArguments()[0];
                        var isValueType = false;
                        var list = new List<string>();
                        if (valueType == typeof(string))
                        {
                            list = value as List<string>;
                        }
                        else if (valueType == typeof(Guid))
                        {
                            if (value is List<Guid> valueList)
                            {
                                foreach (var c in valueList)
                                {
                                    list.Add(c.ToString());
                                }
                            }
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
                                    list.Add(c.ToString(CultureInfo.InvariantCulture));
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
                                    list.Add(c.ToString(CultureInfo.InvariantCulture));
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
                                    list.Add(c.ToString(CultureInfo.InvariantCulture));
                                }
                            }
                        }

                        if (list == null)
                            return;


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
                    }

                    #endregion

                    sqlBuilder.Append(") ");
                }
            }
            else if (exp.Arguments[0].Type.IsArray && exp.Arguments[1] is MemberExpression argExp && argExp.Expression.NodeType == ExpressionType.Parameter)
            {
                sqlBuilder.Append(_queryBody.GetColumnName(argExp));
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
                else if (valueType == "System.Guid[]")
                {
                    if (value is Guid[] valueList)
                    {
                        list = new string[valueList.Length];
                        for (var i = 0; i < valueList.Length; i++)
                        {
                            list[i] = valueList[i].ToString();
                        }
                    }
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
                            list[i] = valueList[i].ToString(CultureInfo.InvariantCulture);
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
                            list[i] = valueList[i].ToString(CultureInfo.InvariantCulture);
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
                            list[i] = valueList[i].ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (list == null)
                    return;

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

                #endregion

                sqlBuilder.Append(") ");
            }
        }

        private void EqualsResolve(MethodCallExpression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp.Object is MemberExpression objExp && objExp.Expression.NodeType == ExpressionType.Parameter)
            {
                sqlBuilder.Append(_queryBody.GetColumnName(objExp));

                string value;
                if (exp.Arguments[0] is ConstantExpression c)
                {
                    value = c.Value.ToString();
                }
                else
                {
                    value = DynamicInvoke(exp.Arguments[0]).ToString();
                }

                sqlBuilder.Append(" = ");
                AppendValue(sqlBuilder, parameters, value);
            }
        }

        private void NotResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null)
                return;

            sqlBuilder.Append("(");

            UnaryResolve(exp, sqlBuilder, parameters);

            sqlBuilder.Append(" = 0)");
        }

        private void MemberInitResolve(Expression exp, StringBuilder sqlBuilder, QueryParameters parameters)
        {
            if (exp == null || !(exp is MemberInitExpression initExp) || !initExp.Bindings.Any())
                return;

            for (var i = 0; i < initExp.Bindings.Count; i++)
            {
                if (initExp.Bindings[i] is MemberAssignment assignment)
                {
                    var descriptor = _queryBody.JoinDescriptors.First(m => m.EntityDescriptor.EntityType == initExp.Type);
                    var col = descriptor.EntityDescriptor.Columns.FirstOrDefault(m => m.PropertyInfo.Name.Equals(assignment.Member.Name));
                    if (col != null)
                    {
                        sqlBuilder.Append(
                            $"{_sqlAdapter.AppendQuote(descriptor.Alias)}.{_sqlAdapter.AppendQuote(col.Name)}");

                        sqlBuilder.Append("=");

                        Resolve(assignment.Expression, sqlBuilder, parameters);

                        if (i < initExp.Bindings.Count - 1)
                            sqlBuilder.Append(",");
                    }
                }
            }
        }

        private object DynamicInvoke(Expression exp)
        {
            var result = Expression.Lambda(exp).Compile().DynamicInvoke();
            return result ?? "";
        }

        #endregion

        private void AppendValue(StringBuilder sqlBuilder, QueryParameters parameters, object value)
        {
            var pName = parameters.Add(value);
            sqlBuilder.Append(_sqlAdapter.AppendParameter(pName));
        }
    }
}
