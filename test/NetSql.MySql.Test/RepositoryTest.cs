using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetSql.Pagination;
using NetSql.SQLite;
using NetSql.Test.Common;
using NetSql.Test.Common.Blog;
using NetSql.Test.Common.Blog.Model;
using NetSql.Test.Common.Blog.Repository;
using Xunit;

namespace NetSql.MySql.Test
{
    public class RepositoryTest
    {
        private readonly IArticleRepository _repository;

        public RepositoryTest()
        {
            var dbContext = new BlogDbContext(new MySqlDbContextOptions("Server=localhost;Database=blog;Uid=root;Pwd=oldli!@#123;Allow User Variables=True;charset=utf8;"));
            _repository = new ArticleRepository(dbContext);
        }

        [Fact]
        public async void AddTest()
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

            await _repository.AddAsync(article);

            Assert.True(article.Id > 0);
        }

        [Fact]
        public async void BatchInsertTest()
        {
            try
            {
                AddTest();
            }
            catch (Exception e)
            {
            }
            var list = new List<Article>();
            for (var i = 0; i < 100; i++)
            {
                var article = new Article
                {
                    CateId = i % 8,
                    Title1 = "test" + i,
                    Summary = "这是一篇测试文章",
                    Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                    ReadCount = 10,
                    IsDeleted = i % 2 == 0,
                    CreatedTime = DateTime.Now,
                };
                list.Add(article);
            }
            var sw = new Stopwatch();
            sw.Start();

            await _repository.AddAsync(list);

            sw.Stop();
            var s = sw.ElapsedMilliseconds;

            Assert.True(s > 0);
        }

        [Fact]
        public async void DeleteTest()
        {
            var b = await _repository.DeleteAsync(2);

            Assert.True(b);
        }

        [Fact]
        public async void UpdateTest()
        {
            var article = await _repository.GetAsync(2);
            article.Title1 = "修改测试";

            var b = await _repository.UpdateAsync(article);

            Assert.True(b);
        }

        [Fact]
        public async void PaginationTest()
        {
            var paging = new Paging(1, 20);
            var list = await _repository.PaginationAsync(paging, m => m.Id > 100);

            Assert.True(paging.TotalCount > 0);
        }
    }
}
