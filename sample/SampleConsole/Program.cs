using System;
using Dapper;
using NetSql.Abstractions.Pagination;
using NetSql.MySql;
using SampleConsole.Domain.Account;
using SampleConsole.Infrastructure.Repositories;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建数据库上下文
            var context = new SampleDbContext(new MySqlDbContextOptions("Sample", "数据库连接字符串，请先创建表，sql文件在Sql目录"));

            //创建仓储
            var accountRepository = new AccountRepository(context);

            var account = new Account
            {
                UserName = "test" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Email = "test",
                Password = "test",
                Phone = "test",
                Creator = Guid.NewGuid(),
                ClosedBy = Guid.Empty,
                ClosedTime = DateTime.Now,
                CreatedTime = DateTime.Now,
                LastLoginIP = "test"
            };

            //添加账户
            var r = accountRepository.AddAsync(account).Result;

            Console.WriteLine(account.Id);

            //查询
            var paging = new Paging();
            var list = accountRepository.Query(paging).Result;
            foreach (var acc in list)
            {
                Console.WriteLine($"ID:{acc.Id},UserName:{acc.UserName}");
            }

            Console.WriteLine($"共查询到{paging.TotalCount}数据");

            //修改
            account = accountRepository.GetAsync(account.Id).Result;
            account.UserName = "demo";
            var c = accountRepository.UpdateAsync(account).Result;
            Console.WriteLine($"修改{account.Id}");

            //删除
            var d = accountRepository.RemoveAsync(account.Id).Result;
            Console.WriteLine($"删除{account.Id}");
        }
    }
}
