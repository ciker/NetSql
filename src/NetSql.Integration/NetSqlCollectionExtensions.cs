using System;
using System.Linq;
using NetSql;
using NetSql.Enums;
using NetSql.Integration;
using NetSql.Repository;
using Oldli.Fw.Utils;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NetSqlCollectionExtensions
    {
        /// <summary>
        /// 注入仓储
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            //获取除系统程序集、Nuget包以外的所有程序及
            var assemblies = AssemblyHelper.GetAssembliesWithImplement(typeof(IRepository<>));

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var interfaceType in interfaces)
                    {
                        if (interfaceType == typeof(IDbContext))
                        {
                            services.AddDbContext(type);
                        }
                        else if (!interfaceType.IsGenericType && interfaceType.GetInterfaces().Any(m => m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IRepository<>)))
                        {
                            services.Add(new ServiceDescriptor(interfaceType, type, ServiceLifetime.Singleton));
                        }
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// 注入数据库上下文
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static void AddDbContext(this IServiceCollection services, Type type)
        {
            foreach (var options in services.BuildServiceProvider().GetService<NetSqlOptionsContext>().OptionsList)
            {
                if (type.Name.StartsWith(options.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var dbContextOptionsAssemblyName = "";
                    var dbContextOptionsTypeName = "";
                    switch (options.DbType)
                    {
                        case DbType.SqlServer:
                            dbContextOptionsAssemblyName = "NetSql";
                            dbContextOptionsTypeName = "DbContextOptions";
                            break;
                        case DbType.MySql:
                            dbContextOptionsAssemblyName = "NetSql.MySql";
                            dbContextOptionsTypeName = "MySqlDbContextOptions";
                            break;
                        case DbType.SQLite:
                            dbContextOptionsAssemblyName = "NetSql.SQLite";
                            dbContextOptionsTypeName = "SQLiteDbContextOptions";
                            break;
                    }

                    var dbContextOptionsType =
                        AssemblyHelper.GetTypeFromAssembly(dbContextOptionsAssemblyName, dbContextOptionsTypeName);

                    services.AddSingleton(type, Activator.CreateInstance(type, Activator.CreateInstance(dbContextOptionsType, options.ConnString)));
                }
            }
        }

        /// <summary>
        /// 注入NetSql
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNetSql(this IServiceCollection services)
        {
            //加载配置
            services.AddSingleton<NetSqlOptionsContext>();

            //注入仓储
            services.AddRepositories();

            return services;
        }
    }
}