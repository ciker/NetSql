using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using NetSql.Core.Entities;

namespace NetSql.Core
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// 
        /// </summary>
        IDbContextOptions Options { get; }

        /// <summary>
        /// 打开一个数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection DbConnection { get; }

        /// <summary>
        /// 打开一个事务
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// 获取实体数据集
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : Entity, new();

        /// <summary>
        /// 从给定的实体类型集合加载数据集
        /// </summary>
        void LoadSets(List<Type> entityTypes);

        /// <summary>
        /// 从指定程序集中初始化数据集
        /// </summary>
        /// <param name="assembly"></param>
        void LoadSetsFromAssembly(Assembly assembly);

    }
}
