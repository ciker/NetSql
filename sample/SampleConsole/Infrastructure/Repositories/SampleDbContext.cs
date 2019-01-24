using NetSql.Abstractions;
using NetSql.Core;

namespace SampleConsole.Infrastructure.Repositories
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(IDbContextOptions options) : base(options)
        {
        }
    }
}
