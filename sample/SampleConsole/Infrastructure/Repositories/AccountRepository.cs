using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetSql.Abstractions;
using NetSql.Abstractions.Pagination;
using NetSql.Core;
using SampleConsole.Domain.Account;

namespace SampleConsole.Infrastructure.Repositories
{
    public class AccountRepository : RepositoryAbstract<Account>, IAccountRepository
    {
        public AccountRepository(IDbContext context) : base(context)
        {
        }

        public Task<IList<Account>> Query(Paging paging, string userName = null, string telephone = null, string email = null)
        {
            var query = Db.Find()
                .WhereIf(!string.IsNullOrWhiteSpace(userName), e => e.UserName.Contains(userName))
                .WhereIf(!string.IsNullOrWhiteSpace(telephone), e => e.Phone.Contains(telephone))
                .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.Contains(email));

            return query.PaginationAsync(paging);
        }
    }
}
