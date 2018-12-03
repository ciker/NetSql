using System;
using Data.Test.Common.Domain.Article;
using Data.Test.Common.Domain.Category;
using Data.Test.Common.Domain.User;
using Data.Test.Common.Infrastructure.Repositories;
using Td.Fw.Data.Core;
using Td.Fw.Data.SQLite;
using Xunit;

namespace Data.Test.SQLite
{
    public class DbSetTests
    {
        private readonly IDbSet<Article> _articleDb;
        private readonly IDbSet<Category> _categoryDb;
        private readonly IDbSet<User> _userDb;

        private readonly Guid _articleId = new Guid("45944e44-84db-4055-84ab-b31a2b66f470");
        public DbSetTests()
        {
            var context = new BlogDbContext(new SQLiteDbContextOptions("Data Source=./Data/Blog.db"));
            _articleDb = context.Set<Article>();
            _categoryDb = context.Set<Category>();
            _userDb = context.Set<User>();
        }

        [Fact]
        public void InsertAndGetTest()
        {
            var category = new Category
            {
                Name = "随笔"
            };

            _categoryDb.InsertAsync(category).Wait();

            var user = new User
            {
                Name = "Oldli",
                Age = 28,
            };

            _userDb.InsertAsync(user).Wait();

            var article = new Article
            {
                Id = _articleId,
                Title = "测试文章",
                Summary = "测试文章测试文章测试文章测试文章测试文章测试文章",
                Body = "测试文章测试文章测试文章测试文章测试文章测试文章测试文章测试文章",
                CategoryId = category.Id,
                Author = user.Id
            };

            _articleDb.InsertAsync(article).Wait();

            Get();
        }

        [Fact]
        public void Get()
        {
            var article = _articleDb.GetAsync(_articleId).Result;

            Assert.NotNull(article);
        }
    }
}
