using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetSql.Test.Common;
using NetSql.Test.Common.Repository;

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

        public TestHostedService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var article = await _articleRepository.GetAsync(1);
            Console.WriteLine(article.Body);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }

}
