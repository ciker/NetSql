using NetSql.Core.Entities;

namespace Data.Test.Domain.User
{
    public class User : EntityWithSoftDelete
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
