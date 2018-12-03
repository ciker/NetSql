﻿using System.Data;
using Td.Fw.Data.Core.Entities;

namespace Td.Fw.Data.Core
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
    }
}
