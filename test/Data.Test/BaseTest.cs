using System;
using System.Data;
using System.IO;
using Data.Test.Infrastructure.Repositories;
using NetSql.Abstractions;
using NetSql.SQLite;

namespace Data.Test
{
    public class BaseTest
    {
        protected IDbContext Ctx;

        protected BaseTest()
        {
            //Ctx = new BlogDbContext(new MySqlDbContextOptions("Blog", "Server=localhost;Database=blog;Uid=root;Pwd=tdkj!@#123;Allow User Variables=True;charset=utf8;SslMode=none;"));

            SQLiteInit();
        }

        private void SQLiteInit()
        {
            var dbName = "./Blog-" + DateTime.Now.ToString("yyyyMMdd") + ".db";
            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }

            Ctx = new BlogDbContext(new SQLiteDbContextOptions("Blog", "Data Source=" + dbName + ";BinaryGUID=false;New=True;"));

            Ctx.Open();

            var cmd = Ctx.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = @"CREATE TABLE Article (
                                Id UNIQUEIDENTIFIER PRIMARY KEY,  
                                CategoryId UNIQUEIDENTIFIER NOT NULL,  
                                Title TEXT NOT NULL,  
                                Summary TEXT NOT NULL,  
                                Body TEXT NOT NULL,  
                                CreatedTime datetime NOT NULL, 
                                ReadCount integer NOT NULL DEFAULT 0, 
                                IsDeleted bit NOT NULL DEFAULT 0,  
                                DeletedTime datetime NOT NULL,  
                                Deleter UNIQUEIDENTIFIER NOT NULL,  
                                Author UNIQUEIDENTIFIER NOT NULL)";

            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE Category (
                                Id UNIQUEIDENTIFIER PRIMARY KEY,
                                Name TEXT NOT NULL);";

            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE User (
                                Id UNIQUEIDENTIFIER PRIMARY KEY,
                                Name TEXT NOT NULL,
                                Age integer NOT NULL DEFAULT 0,
                                Gender integer NOT NULL DEFAULT 0,
                                IsDeleted bit NOT NULL DEFAULT 0,  
                                DeletedTime datetime NOT NULL,  
                                Deleter UNIQUEIDENTIFIER NOT NULL);";

            cmd.ExecuteNonQuery();

            Ctx.Connection.Close();
        }
    }
}
