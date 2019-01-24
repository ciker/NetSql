using System.Collections.Generic;
using Dapper;

namespace NetSql.Core.SqlQueryable.Internal
{
    /// <summary>
    /// 参数集合
    /// </summary>
    public class QueryParameters
    {
        private readonly List<KeyValuePair<string, object>> _parameters;

        public QueryParameters()
        {
            _parameters = new List<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// 添加参数，返回参数名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Add(object value)
        {
            var paramName = "P" + (_parameters.Count + 1);
            _parameters.Add(new KeyValuePair<string, object>(paramName, value));
            return paramName;
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <returns></returns>
        public DynamicParameters Parse()
        {
            var dynParams = new DynamicParameters();

            _parameters.ForEach(m =>
            {
                dynParams.Add(m.Key, m.Value);
            });

            return dynParams;
        }
    }
}
