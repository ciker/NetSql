using System.Linq;
using NetSql.Pagination;

namespace NetSql.Query
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

        /// <summary>
        /// 转换为Paging对象
        /// </summary>
        /// <returns></returns>
        public Paging Paging()
        {
            if (Page == null)
                return null;

            var paging = new Paging(Page.Index, Page.Size);
            if (Page.Sort != null && Page.Sort.Any())
            {
                foreach (var sort in Page.Sort)
                {
                    paging.OrderBy.Add(new Sort(sort.Field, sort.Type));
                }
            }

            return paging;
        }
    }
}
