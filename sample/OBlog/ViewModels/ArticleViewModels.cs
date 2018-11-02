using NetSql.Query;

namespace OBlog.ViewModels
{
    public class ArticleCreateModel
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
    }

    public class ArticleQueryModel : QueryModel
    {
        public string Title { get; set; }
    }
}
