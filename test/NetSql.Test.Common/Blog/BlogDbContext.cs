using NetSql.Test.Common.Blog.Model;

namespace NetSql.Test.Common.Blog
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(IDbContextOptions options) : base(options)
        {
        }

        public IDbSet<Article> Articles { get; set; }

        public IDbSet<User> User { get; set; }

        public IDbSet<Category> Category { get; set; }
    }
}
