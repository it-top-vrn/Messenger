using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace DB_API
{
    class DBContext : DbContext
    {
        public DbSet<ClientTable> Client_table { get; set; }
        public DbSet<ContactTable> Contact_table { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public static DBContext Init()
        {
            var builder = new ConfigurationBuilder();
            /*var connectionString = builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("DefaultConnection");*/
            var connectionString = "server=mysql60.hostland.ru;uid=host1323541_itstep;pwd=269f43dc;database=host1323541_vrn05;";
            var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
            var options = optionsBuilder.UseMySQL(connectionString).Options;
            return new DBContext(options);
        }

    }
}
