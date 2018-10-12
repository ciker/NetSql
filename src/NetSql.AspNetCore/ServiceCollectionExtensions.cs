using NetSql;
using NetSql.Repository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryServiceCollectionExtensions
    {
        /// <summary>
        /// 添加NetSql服务
        /// </summary>
        /// <typeparam name="T">IDbContext实现</typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns></returns>
        public static IServiceCollection AddNetSql<T>(this IServiceCollection services, string connectionString) where T : IDbContext
        {
            //数据库上下文
            services.AddSingleton<IDbContextOptions>(new DbContextOptions(connectionString));
            services.AddSingleton(typeof(IDbContext), typeof(T));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

    }
}
