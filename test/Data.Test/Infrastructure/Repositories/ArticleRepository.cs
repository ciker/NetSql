using Data.Test.Domain.Article;
using NetSql.Abstractions;
using NetSql.Core;

namespace Data.Test.Infrastructure.Repositories
{
    public class ArticleRepository : RepositoryAbstract<Article>, IArticleRepository
    {
        public ArticleRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
