using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using NetSql;
using NetSql.Enums;
using NetSql.Integration;
using NetSql.Repository;

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
            //获取除系统程序集、Nuget包以外的所有程序集
            var assemblies = DependencyContext.Default.CompileLibraries.Where(m => !m.Serviceable && m.Type == "project").Select(m => AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(m.Name))).ToList();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var interfaceType in interfaces)
                    {
                        if (interfaceType == typeof(IDbContext) && type != typeof(DbContext))
                        {
                            services.AddDbContext(type);
                        }
                        else if (!interfaceType.IsGenericType && interfaceType.GetInterfaces().Any(m => m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IRepository<>)))
                        {
                            services.TryAdd(new ServiceDescriptor(interfaceType, type, ServiceLifetime.Singleton));
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
            var optionsList = services.BuildServiceProvider().GetService<NetSqlOptionsContext>().OptionsList;
            foreach (var options in optionsList)
            {
                //约定配置项的名称与上下文类名称头相同
                if (type.Name.StartsWith(options.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var dbContextOptionsAssemblyName = "";
                    var dbContextOptionsTypeName = "";
                    switch (options.DbType)
                    {
                        case DatabaseType.SqlServer:
                            dbContextOptionsAssemblyName = "NetSql";
                            dbContextOptionsTypeName = "DbContextOptions";
                            break;
                        case DatabaseType.MySql:
                            dbContextOptionsAssemblyName = "NetSql.MySql";
                            dbContextOptionsTypeName = "MySqlDbContextOptions";
                            break;
                        case DatabaseType.SQLite:
                            dbContextOptionsAssemblyName = "NetSql.SQLite";
                            dbContextOptionsTypeName = "SQLiteDbContextOptions";
                            break;
                    }

                    var dbContextOptionsType = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(dbContextOptionsAssemblyName)).GetType($"{dbContextOptionsAssemblyName}.{dbContextOptionsTypeName}");
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