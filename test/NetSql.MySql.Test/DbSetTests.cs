using System;
using System.Diagnostics;
using System.Linq;
using NetSql.Enums;
using NetSql.SQLite;
using NetSql.Test.Common;
using NetSql.Test.Common.Model;
using Xunit;

namespace NetSql.MySql.Test
{
    public class DbSetTests
    {
        private readonly BlogDbContext _dbContext;
        private readonly IDbSet<Article> _dbArticle;
        private readonly IDbSet<User> _dbUser;

        public DbSetTests()
        {
            //_dbContext = new BlogDbContext(new SQLiteDbContextOptions("Filename=./Database/Test.db"));
            _dbContext = new BlogDbContext(new DbContextOptions("Server=.;Initial Catalog=NetSqlTest;User ID=sa;Password=oldli!@#123"));
            _dbArticle = _dbContext.Set<Article>();
            _dbUser = _dbContext.Set<User>();

            //预热
            //_dbArticle.Find().First();
        }

        [Fact]
        public async void InsertTest()
        {
            var article = new Article
            {
                Title1 = "test",
                Summary = "这是一篇测试文章",
                Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                ReadCount = 10,
                IsDeleted = true,
                CreatedTime = DateTime.Now
            };

            await _dbArticle.InsertAsync(article);

            Assert.True(article.Id > 0);
        }

        [Fact]
        public async void BatchInsertTest()
        {
            await _dbUser.InsertAsync(new User { Name = "oldli" });
            await _dbUser.InsertAsync(new User { Name = "admin" });

            var sw = new Stopwatch();
            sw.Start();

            var tran = _dbContext.BeginTransaction();

            for (var i = 0; i < 1000; i++)
            {
                var article = new Article
                {
                    Title1 = "test" + i,
                    Summary = "这是一篇测试文章",
                    Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                    ReadCount = 10,
                    IsDeleted = i % 2 == 0,
                    CreatedTime = DateTime.Now,
                    UserID = i % 2 + 1
                };

                await _dbArticle.InsertAsync(article, tran);
            }

            tran.Commit();

            sw.Stop();

            var s = sw.ElapsedMilliseconds;

            Assert.True(s > 0);
        }

        [Fact]
        public void DeleteTest()
        {
            var b = _dbArticle.DeleteAsync(3).Result;

            Assert.True(b);
        }

        [Fact]
        public async void DeleteWhereTest()
        {
            var b = await _dbArticle.Find(m => m.Id > 10 && m.Id < 15).Delete();

            Assert.True(b);
        }

        [Fact]
        public async void UpdateTest()
        {
            var article = await _dbArticle.Find().First();
            article.Title1 = "修改测试";

            var b = await _dbArticle.UpdateAsync(article);

            Assert.True(b);
        }

        [Fact]
        public async void UpdateWhereTest()
        {
            var b = await _dbArticle.Find(m => m.Id < 100).Update(m => new Article
            {
                Title1 = "hahahaah",
                ReadCount = 1000
            });

            Assert.True(b);
        }

        [Fact]
        public async void GetTest()
        {
            var article = await _dbArticle.GetAsync(100);

            Assert.NotNull(article);
        }

        [Fact]
        public async void GetWehreTest()
        {
            var article = await _dbArticle.Find().Where(m => m.Id > 1).First();

            Assert.NotNull(article);
        }

        [Fact]
        public async void FindTest()
        {
            var list = await _dbArticle.Find(m => m.Id > 100 && m.Id < 120).ToList();

            Assert.Equal(19, list.Count);
        }

        [Theory]
        [InlineData(2)]
        public async void WhereTest(int id)
        {
            var query = _dbArticle.Find().WhereIf(id > 1, m => m.Id > 200);
            var sql = query.ToSql();
            var list = await query.ToList();

            Assert.Equal(99, list.Count);
        }

        [Fact]
        public async void OrderByTest()
        {
            var query = _dbArticle.Find(m => m.Id > 200 && m.Id < 1000).OrderByDescending(m => m.Id);
            var list = await query.ToList();

            Assert.Equal(99, list.Count);
        }

        [Fact]
        public async void FirstTest()
        {
            var query = _dbArticle.Find(m => m.Id > 100 && m.Id < 120);
            var sql = query.ToSql();
            var article = await query.First();
            Assert.NotNull(article);
        }

        [Fact]
        public async void LimitTest()
        {
            var query = _dbArticle.Find(m => m.Id > 100 && m.Id < 120).Limit(5, 10);
            var sql = query.ToSql();
            var list = await query.ToList();

            Assert.Equal(10, list.Count);
        }

        [Fact]
        public async void MaxTest()
        {
            var maxReadCount = await _dbArticle.Find().Max(m => m.ReadCount);

            Assert.True(maxReadCount > 0);
        }

        [Fact]
        public void MinTest()
        {
            var maxReadCount = _dbArticle.Find().Min(m => m.ReadCount).Result;

            Assert.True(maxReadCount > 0);
        }

        [Fact]
        public void CountTest()
        {
            var sw = new Stopwatch();
            sw.Start();

            var count = _dbArticle.Find(m => m.Id > 100).Count().Result;

            sw.Stop();

            Assert.True(count > 0);
        }

        [Fact]
        public void InTest()
        {
            var ids = new[] { 100, 200 };
            var list = _dbArticle.Find(m => ids.Contains(m.Id)).ToList().Result;

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void StartsWithTest()
        {
            var list = _dbArticle.Find(m => m.Title1.StartsWith("test11")).ToList().Result;

            Assert.NotEmpty(list);
        }

        [Fact]
        public void EndsWithTest()
        {
            var list = _dbArticle.Find(m => m.Title1.EndsWith("11")).ToList().Result;

            Assert.NotEmpty(list);
        }

        [Fact]
        public void ContainsTest()
        {
            var list = _dbArticle.Find(m => m.Title1.Contains("11")).ToList().Result;

            Assert.NotEmpty(list);
        }

        [Fact]
        public async void EqualsTest()
        {
            var query = _dbArticle.Find(m => m.Id.Equals(1));
            var sql = query.ToSql();
            var list = await query.ToList();

            Assert.NotEmpty(list);
        }

        [Fact]
        public async void SelectTest()
        {
            var query = _dbArticle.Find().Select(m => new { m.Id, m.Title1 }).Limit(0, 10);
            var list = await query.ToList();

            Assert.NotEmpty(list);
        }

        [Fact]
        public void LeftJoinTest()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < 10000; i++)
            {
                var query = _dbArticle.InnerJoin<User>((m, n) => m.UserID == n.Id)
                    .LeftJoin<Tag>((t1, t2, t3) => t1.TagID == t3.Id)
                    .LeftJoin<Category>((t1, t2, t3, t4) => t1.CateID == t4.Id)
                    .LeftJoin<Commit>((t1, t2, t3, t4, t5) => t1.CateID == t5.Id)
                    .Select((t1, t2, t3, t4, t5) => new
                    {
                        t1.Id,
                        t1.Title1,
                        UserName = t2.Name,
                        TagName = t3.Name,
                        CateName = t4.Name,
                        CommitContent = t5.Content
                    });

                var sql = query.ToSql();
            }

            sw.Start();

            var s = sw.ElapsedMilliseconds;
        }
    }

    public class Test
    {
        public int Id { get; set; }

        public string UserName { get; set; }
    }
}
