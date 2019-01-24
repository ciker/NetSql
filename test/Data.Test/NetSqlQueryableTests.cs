using Data.Test.Domain.Article;
using NetSql.Abstractions;
using Xunit;

namespace Data.Test
{
    public class NetSqlQueryableTests : BaseTest
    {
        private readonly IDbSet<Article> _articleDb;

        public NetSqlQueryableTests()
        {
            //MySql
            _articleDb = Ctx.Set<Article>();
        }

        [Fact]
        public async void WhereTest()
        {
            var model = await _articleDb.Find().FirstAsync();

            Assert.NotNull(model);
        }

    }
}
