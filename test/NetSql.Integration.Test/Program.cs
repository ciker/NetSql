using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetSql.Test.Common.Account.Model;
using NetSql.Test.Common.Account.Repositories;
using NetSql.Test.Common.Blog.Repository;

namespace NetSql.Integration.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            new HostBuilder()
                .ConfigureAppConfiguration((context, cfg) =>
                {
                    cfg.SetBasePath(Directory.GetCurrentDirectory());
                    cfg.AddJsonFile("appsettings.json", true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddNetSql();

                    services.AddHostedService<TestHostedService>();

                }).Build().Run();
        }
    }

    public class TestHostedService : IHostedService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IAccountRepository _accountRepository;

        public TestHostedService(IArticleRepository articleRepository, IAccountRepository accountRepository)
        {
            _articleRepository = articleRepository;
            _accountRepository = accountRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var article = await _articleRepository.GetAsync(1);
            Console.WriteLine(article.Body);

            await _accountRepository.AddAsync(new Account { Password = "hahaahah", UserName = "laoli" });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}
