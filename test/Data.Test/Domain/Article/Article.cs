using System;
using NetSql.Abstractions.Attributes;
using NetSql.Core.Entities;

namespace Data.Test.Domain.Article
{
    public class Article : EntityWithSoftDelete
    {
        public Guid CategoryId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Body { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public int ReadCount { get; set; }

        public Guid Author { get; set; }

        [Ignore]
        public string AuthorName { get; set; }


        [Ignore]
        public string CategoryName { get; set; }
    }
}
