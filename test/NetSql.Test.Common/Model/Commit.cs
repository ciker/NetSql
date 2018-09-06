using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Entities;

namespace NetSql.Test.Common.Model
{
    public class Commit : EntityBase
    {
        public string Content { get; set; }
    }
}
