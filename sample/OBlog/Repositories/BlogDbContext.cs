using NetSql;
using OBlog.Domain;

namespace OBlog.Infrastructure.Repositories
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(IDbContextOptions options) : base(options)
        {
        }

        public IDbSet<Article> Article { get; set; }
    }
}
