using Data.Test.Infrastructure.Repositories;
using NetSql.Abstractions;
using NetSql.MySql;

namespace Data.Test
{
    public class BaseTest
    {
        protected IDbContext Ctx;

        protected BaseTest()
        {
            Ctx= new BlogDbContext(new MySqlDbContextOptions("Blog", "Server=localhost;Database=blog;Uid=root;Pwd=123456;Allow User Variables=True;charset=utf8;SslMode=none;"));
        }
    }
}
