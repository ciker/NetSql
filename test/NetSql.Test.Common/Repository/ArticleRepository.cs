﻿using NetSql.Repository;
using NetSql.Test.Common.Model;

namespace NetSql.Test.Common.Repository
{
    public class ArticleRepository : RepositoryAbstract<Article>, IArticleRepository
    {
        public ArticleRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
