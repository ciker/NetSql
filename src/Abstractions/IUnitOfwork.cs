﻿using System.Data;

namespace NetSql.Abstractions
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="isolationLevel">隔离级别</param>
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        void Commit();

        /// <summary>
        /// 回滚
        /// </summary>
        void Rollback();
    }

    /// <summary>
    /// 工作单元泛型
    /// </summary>
    public interface IUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : IDbContext
    {

    }
}
