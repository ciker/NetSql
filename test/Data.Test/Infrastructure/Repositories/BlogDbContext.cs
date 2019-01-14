using NetSql.Abstractions;
using NetSql.Core;

namespace Data.Test.Infrastructure.Repositories
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(IDbContextOptions options) : base(options)
        {
        }
    }
}
