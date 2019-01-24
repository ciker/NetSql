using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetSql.Abstractions.Pagination;

namespace SampleConsole.Domain.Account
{
    public interface IAccountRepository
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="userName"></param>
        /// <param name="telephone"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<IList<Account>> Query(Paging paging, string userName = null, string telephone = null, string email = null);
    }
}
