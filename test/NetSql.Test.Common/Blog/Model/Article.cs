using System;
using NetSql.Entities;
using NetSql.Mapper;

namespace NetSql.Test.Common.Blog.Model
{
    [Table("b_article")]
    public class Article : EntityBase
    {
        public int UserId { get; set; }

        public int CateId { get; set; }

        [Column("Title")]
        public string Title1 { get; set; }

        public string Summary { get; set; }

        public string Body { get; set; }

        public int ReadCount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedTime { get; set; }

    }
}
