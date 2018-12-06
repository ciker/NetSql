using System;
using Data.Test.Common.Domain.Article;
using Data.Test.Common.Domain.Category;
using Data.Test.Common.Domain.User;
using Data.Test.Common.Infrastructure.Repositories;
using Xunit;

namespace Data.Test.SqlServer
{
    public class DbSetTests
    {
        private readonly IDbSet<Article> _articleDb;
        private readonly IDbSet<Category> _categoryDb;
        private readonly IDbSet<User> _userDb;

        private readonly Guid _articleId = new Guid("45944e44-84db-4055-84ab-b31a2b66f470");
        public DbSetTests()
        {
            var context = new BlogDbContext(new SqlServerDbContextOptions("Server=1.1.1.3;Initial Catalog=Blog;User ID=sa;Password=oldli!@#123"));
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

        public static Guid GenerateGuid()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.Now;
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = now.TimeOfDay;

            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}
