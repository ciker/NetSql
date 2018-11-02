using System;
using NetSql.Entities;

namespace OBlog.Domain
{
    /// <summary>
    /// 文章
    /// </summary>
    public class Article : EntityBase
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

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }

        public int ReadCount { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
