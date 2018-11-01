using NetSql.Repository;
using NetSql.Test.Common.Blog.Model;

namespace NetSql.Test.Common.Blog.Repository
{
    public class UserRepository : RepositoryAbstract<User>, IUserRepository
    {
        public UserRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
