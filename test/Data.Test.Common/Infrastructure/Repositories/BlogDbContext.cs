using Data.Test.Common.Domain.Article;
using Data.Test.Common.Domain.Category;
using Data.Test.Common.Domain.User;
using Td.Fw.Data.Core;

namespace Data.Test.Common.Infrastructure.Repositories
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(IDbContextOptions options) : base(options)
        {
        }


        public IDbSet<Article> Article { get; set; }

        public IDbSet<User> User { get; set; }

        public IDbSet<Category> Category { get; set; }
    }
}
