using NetSql.Test.Common.Model;

namespace NetSql.Test.Common
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(IDbContextOptions options) : base(options)
        {
        }

        public IDbSet<Article> Articles { get; set; }
        public IDbSet<User> User { get; set; }
        public IDbSet<Tag> Tag { get; set; }

        public IDbSet<Category> Category { get; set; }

        public IDbSet<Commit> Commit { get; set; }
    }
}
