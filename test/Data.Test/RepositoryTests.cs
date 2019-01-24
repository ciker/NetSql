using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Data.Test.Domain.Article;
using Data.Test.Infrastructure.Repositories;
using Xunit;

namespace Data.Test
{
    public class RepositoryTests : BaseTest
    {
        private readonly IArticleRepository _articleRepository;

        public RepositoryTests()
        {
            _articleRepository = new ArticleRepository(Ctx);
        }

        [Fact]
        public async Task<Guid> AddAsyncTest()
        {
            var article = new Article
            {
                Title = "测试文章",
                Summary = "测试文章测试文章测试文章测试文章测试文章测试文章",
                Body = "测试文章测试文章测试文章测试文章测试文章测试文章测试文章测试文章",
                CategoryId = Guid.NewGuid(),
                Author = Guid.NewGuid(),
            };

            var result = await _articleRepository.AddAsync(article);

            Assert.True(result);

            return article.Id;
        }

        [Fact]
        public async Task BatchAddAsyncTest()
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

            var result = await _articleRepository.AddAsync(list);

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
        public async void ExistsAsyncTest()
        {
            var id = await AddAsyncTest();

            var result = await _articleRepository.ExistsAsync(m => m.Id == id);
            Assert.True(result);
        }

        [Fact]
        public async void RemoveAsyncTest()
        {
            var id = await AddAsyncTest();

            var result = await _articleRepository.ExistsAsync(m => m.Id == id);
            Assert.True(result);

            await _articleRepository.RemoveAsync(id);
            var result1 = await _articleRepository.ExistsAsync(m => m.Id == id);
            Assert.False(result1);
        }

        [Fact]
        public async void UpdateAsyncTest()
        {
            var id = await AddAsyncTest();
            var article = await _articleRepository.GetAsync(id);
            article.Title = "修改测试" + DateTime.Now;
            await _articleRepository.UpdateAsync(article);

            var newArticle = await _articleRepository.GetAsync(id);

            Assert.Equal(newArticle.Title, article.Title);
        }

        [Fact]
        public async void GetAllAsyncTest()
        {
            var list = await _articleRepository.GetAllAsync();
            Assert.NotEmpty(list);
        }
    }
}
