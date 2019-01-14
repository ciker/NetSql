using Data.Test.Domain.User;
using NetSql.Abstractions;
using NetSql.Core;

namespace Data.Test.Infrastructure.Repositories
{
    public class UserRepository : RepositoryAbstract<User>, IUserRepository
    {
        public UserRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
