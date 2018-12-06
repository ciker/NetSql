using System.Data;

namespace Td.Fw.Data.Core.Repository
{
    public interface IUnitOfWork<TDbContext> where TDbContext : IDbContext
    {
        /// <summary>
        /// 打开一个事务
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

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
}
