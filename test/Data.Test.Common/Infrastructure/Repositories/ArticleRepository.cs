using Data.Test.Common.Domain.Article;
using Td.Fw.Data.Core.Repository;

namespace Data.Test.Common.Infrastructure.Repositories
{
    public class ArticleRepository : RepositoryAbstract<Article>, IArticleRepository
    {
        public ArticleRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
