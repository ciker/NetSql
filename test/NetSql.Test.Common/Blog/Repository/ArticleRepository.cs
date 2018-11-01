using NetSql.Repository;
using NetSql.Test.Common.Blog.Model;

namespace NetSql.Test.Common.Blog.Repository
{
    public class ArticleRepository : RepositoryAbstract<Article>, IArticleRepository
    {
        public ArticleRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
