using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Repository;

namespace NetSql.Test.Common.Account.Repositories
{
    public class AccountRepository : RepositoryAbstract<Model.Account>, IAccountRepository
    {
        public AccountRepository(AccountDbContext dbContext) : base(dbContext)
        {
        }
    }
}
