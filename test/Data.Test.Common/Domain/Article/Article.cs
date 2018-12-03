using System;
using Td.Fw.Data.Core.Entities;

namespace Data.Test.Common.Domain.Article
{
    public class Article : EntityBaseWithSoftDelete
    {
        public Guid CategoryId { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Body { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public int ReadCount { get; set; }

        public Guid Author { get; set; }
    }
}
