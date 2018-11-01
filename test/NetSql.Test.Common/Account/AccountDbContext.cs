using System;
using System.Collections.Generic;
using System.Text;

namespace NetSql.Test.Common.Account
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(IDbContextOptions options) : base(options)
        {
        }

        public IDbSet<Model.Account> Account { get; set; }
    }
}
