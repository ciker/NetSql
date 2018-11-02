using System;
using NetSql.Query;

namespace OBlog.ResultModels
{
    public class ArticleQueryResultModel: QueryResultModel<ArticleQueryItemModel>
    {
    }

    public class ArticleQueryItemModel
    {

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }

        public int ReadCount { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
