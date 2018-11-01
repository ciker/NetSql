using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Entities;

namespace NetSql.Test.Common.Account.Model
{
    public class Account : EntityBase
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
