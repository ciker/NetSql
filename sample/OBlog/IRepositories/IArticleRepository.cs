using System.Collections.Generic;
using System.Threading.Tasks;
using NetSql.Pagination;
using NetSql.Repository;

namespace OBlog.Domain.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IList<Article>> Query(string title, Paging paging);
    }
}
