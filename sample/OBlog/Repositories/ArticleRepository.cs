using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using NetSql.Pagination;
using NetSql.Repository;
using OBlog.Domain;
using OBlog.Domain.Repositories;

namespace OBlog.Infrastructure.Repositories
{
    public class ArticleRepository : RepositoryAbstract<Article>, IArticleRepository
    {
        public ArticleRepository(BlogDbContext dbContext) : base(dbContext)
        {
        }

        public Task<IList<Article>> Query(string title, Paging paging)
        {
            var query = Db.Find().WhereIf(!string.IsNullOrWhiteSpace(title), m => m.Title.Contains(title));
            query.Select(m => new
            {
                m.Id,
                m.Title,
                m.Summary,
                m.ReadCount,
                m.CreatedTime
            });

            if (!paging.OrderBy.Any())
                query.OrderByDescending(m => m.Id);

            return PaginationAsync(paging, query);
        }

    }
}
