using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OBlog.Infrastructure.ObjectMapper
{
    public static class TinyMapperServiceCollectionExtensions
    {
        /// <summary>
        /// 从指定类型所在的程序集注册模型映射
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            var interfaceType = typeof(IObjectMapper);
            var types = Assembly.GetCallingAssembly().GetTypes().Where(m => interfaceType.IsAssignableFrom(m) && interfaceType != m);
            foreach (var type in types)
            {
                ((IObjectMapper)Activator.CreateInstance(type)).Mapper();
            }

            return services;
        }
    }
}
