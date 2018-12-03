using System;
using System.Collections.Generic;
using System.Text;
using Td.Fw.Data.Core.Entities;

namespace Data.Test.Common.Domain.User
{
    public class User : EntityBaseWithSoftDelete
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

    }

    public enum Gender
    {
        Boy,
        Girl
    }
}
