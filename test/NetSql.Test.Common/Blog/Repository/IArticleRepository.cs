using NetSql.Repository;
using NetSql.Test.Common.Blog.Model;

namespace NetSql.Test.Common.Blog.Repository
{
    public interface IArticleRepository : IRepository<Article>
    {
    }
}
