using Data.Test.Domain.Article;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Test.Infrastructure.Repositories;
using NetSql.Abstractions;
using NetSql.Core;
using Xunit;

namespace Data.Test
{
    public class UnitOfWorkTests : BaseTest
    {
        private IUnitOfWork _uow;
        private readonly IArticleRepository _articleRepository;
        public UnitOfWorkTests()
        {
            _uow = new UnitOfWork(Ctx);
            _articleRepository = new ArticleRepository(Ctx);
        }

        [Fact]
        public async void CommitTestAsync()
        {
            _uow.BeginTransaction();
            var article = new Article
            {
                Title = "测试文章",
                Summary = "测试文章测试文章测试文章测试文章测试文章测试文章",
                Body = "测试文章测试文章测试文章测试文章测试文章测试文章测试文章测试文章",
                CategoryId = Guid.NewGuid(),
                Author = Guid.NewGuid(),
            };

            await _articleRepository.AddAsync(article);

            _uow.Commit();

            var entity = await _articleRepository.GetAsync(article.Id);

            Assert.NotNull(entity);
        }

        [Fact]
        public async Task RollbackTestAsync()
        {
            _uow.BeginTransaction();
            var article = new Article
            {
                Title = "测试文章",
                Summary = "测试文章测试文章测试文章测试文章测试文章测试文章",
                Body = "测试文章测试文章测试文章测试文章测试文章测试文章测试文章测试文章",
                CategoryId = Guid.NewGuid(),
                Author = Guid.NewGuid(),
            };

            var result = await _articleRepository.AddAsync(article);

            _uow.Rollback();
            var entity = await _articleRepository.GetAsync(article.Id);
            Assert.Null(entity);

            //return article.Id;
        }

        [Fact]
        public async Task MoreCommitTestAsync()
        {
            CommitTestAsync();
            CommitTestAsync();
            //return article.Id;
        }
    }
}
