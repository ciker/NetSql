using Data.Test.Common.Domain.Category;
using Td.Fw.Data.Core.Repository;

namespace Data.Test.Common.Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryAbstract<Category>, ICategoryRepository
    {
        public CategoryRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
