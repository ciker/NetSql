using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Repository;
using NetSql.Test.Common.Model;

namespace NetSql.Test.Common.Repository
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
