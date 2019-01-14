using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Data.Test.Domain.Article;
using Data.Test.Domain.Category;
using Data.Test.Domain.User;
using NetSql.Abstractions;
using Xunit;

namespace Data.Test
{
    public class DbSetTests : BaseTest
    {
        private readonly IDbSet<Article> _articleDb;
        private readonly IDbSet<Category> _categoryDb;
        private readonly IDbSet<User> _userDb;

        public DbSetTests()
        {
            //MySql
            _articleDb = Ctx.Set<Article>();
            _categoryDb = Ctx.Set<Category>();
            _userDb = Ctx.Set<User>();
        }

        [Fact]
        public async Task<Guid> InsertAsyncTest()
        {
            var article = new Article
            {
                Title = "测试文章",
                Summary = "测试文章测试文章测试文章测试文章测试文章测试文章",
                Body = "测试文章测试文章测试文章测试文章测试文章测试文章测试文章测试文章",
                CategoryId = Guid.NewGuid(),
                Author = Guid.NewGuid(),
            };

            var result = await _articleDb.InsertAsync(article);

            Assert.True(result);

            return article.Id;
        }

        [Fact]
        public async void BatchInsertAsyncTest()
        {
            var list = new List<Article>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new Article
                {
                    Title = "测试文章" + i,
                    Summary = "测试文章测试文章测试文章测试文章测试文章测试文章",
                    Body = "测试文章测试文章测试文章测试文章测试文章测试文章测试文章测试文章",
                    CategoryId = Guid.NewGuid(),
                    Author = Guid.NewGuid(),
                });
            }

            var sw = new Stopwatch();
            sw.Start();

            var result = await _articleDb.BatchInsertAsync(list);

            sw.Stop();

            /*
             * 耗时统计，1w数据：
             *
             * MySql：1010ms
             */
            var useTime = sw.ElapsedMilliseconds;

            Assert.True(result);
        }

        [Fact]
        public async void GetAsyncTest()
        {
            var id = await InsertAsyncTest();
            var article = await _articleDb.GetAsync(id);

            Assert.NotNull(article);
            Assert.Equal(article.Id, id);
        }

        [Fact]
        public async void DeleteAsyncTest()
        {
            var id = await InsertAsyncTest();

            await _articleDb.DeleteAsync(id);

            var articleInfo = await _articleDb.GetAsync(id);

            Assert.Null(articleInfo);
        }

        [Fact]
        public async void SoftDeleteAsyncTest()
        {
            var id = await InsertAsyncTest();

            var deletor = Guid.NewGuid();
            await _articleDb.SoftDeleteAsync(id, deletor);

            var article = await _articleDb.GetAsync(id);

            Assert.NotNull(article);
            Assert.True(article.IsDeleted);
            Assert.Equal(article.Deletor, deletor);
        }

        [Fact]
        public async void UpdateAsyncTest()
        {
            var id = await InsertAsyncTest();

            var oldArticle = await _articleDb.GetAsync(id);
            oldArticle.Title = "修改测试";
            await _articleDb.UpdateAsync(oldArticle);

            var newArticle = await _articleDb.GetAsync(id);

            Assert.Equal(newArticle.Title, oldArticle.Title);
        }

        [Fact]
        public async void FindAsyncTest()
        {
            var id = await InsertAsyncTest();
            var query = _articleDb.Find(m => m.Id == id);
            var article = await query.FirstAsync();

            Assert.NotNull(article);
            Assert.Equal(id, article.Id);
        }

        [Fact]
        public async void LeftJoinTest()
        {
            var user = new User
            {
                Name = "oldli",
                Age = 20,
                Gender = Gender.Boy
            };
            await _userDb.InsertAsync(user);

            var category = new Category
            {
                Name = "随笔"
            };
            await _categoryDb.InsertAsync(category);

            var article = new Article
            {
                Title = "测试文章",
                Summary = "测试文章测试文章测试文章测试文章测试文章测试文章",
                Body = "测试文章测试文章测试文章测试文章测试文章测试文章测试文章测试文章",
                CategoryId = category.Id,
                Author = user.Id
            };

            await _articleDb.InsertAsync(article);

            var list = await _articleDb.LeftJoin<User>((t1, t2) => t1.Author == t2.Id)
                .LeftJoin<Category>((t1, t2, t3) => t1.CategoryId == t3.Id).Where((t1, t2, t3) => t1.Id == article.Id)
                .Select((t1, t2, t3) => new
                {
                    t1,
                    AuthorName = t2.Name,
                    CategoryName = t3.Name
                })
                .ToListAsync();

            Assert.NotEmpty(list);
            Assert.Equal(list[0].AuthorName, user.Name);
            Assert.Equal(list[0].CategoryName, category.Name);
        }
    }
}
