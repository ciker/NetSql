using System;
using System.Collections.Generic;
using System.Text;
using Data.Test.Common.Domain.User;
using Td.Fw.Data.Core;
using Td.Fw.Data.Core.Repository;

namespace Data.Test.Common.Infrastructure.Repositories
{
    public class UserRepository : RepositoryAbstract<User>, IUserRepository
    {
        public UserRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
