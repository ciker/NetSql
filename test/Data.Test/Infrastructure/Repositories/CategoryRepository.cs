using Data.Test.Domain.Category;
using NetSql.Abstractions;
using NetSql.Core;

namespace Data.Test.Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryAbstract<Category>, ICategoryRepository
    {
        public CategoryRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
