﻿namespace NetSql.Query
{
    /// <summary>
    /// 查询抽象
    /// </summary>
    public class QueryModel
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public QueryPagingModel Page { get; set; } = new QueryPagingModel();
    }
}
