using System;
using System.Data;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Core.Entities;

namespace NetSql.Core
{
    public abstract class DbContextOptionsAbstract : IDbContextOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">连接名称</param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="sqlAdapter">数据库适配器</param>
        protected DbContextOptionsAbstract(string name, string connectionString, ISqlAdapter sqlAdapter)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "数据库连接名称未配置");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "数据库连接字符串未配置");

            Name = name;
            ConnectionString = connectionString;
            SqlAdapter = sqlAdapter;
            EntityDescriptorCollection = new EntityDescriptorCollection(SqlAdapter, new EntitySqlBuilder());
        }

        public string Name { get; }

        public ISqlAdapter SqlAdapter { get; }

        public string ConnectionString { get; }

        public IEntityDescriptorCollection EntityDescriptorCollection { get; }

        public abstract IDbConnection OpenConnection();
    }
}
