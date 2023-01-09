// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.TestModels.JsonQuery;
using Microsoft.Extensions.Options;

namespace Microsoft.EntityFrameworkCore.TestModels.JsonTest
{
    public class JsonTestSqliteContext : DbContext
    {
        private static readonly string _baseDirectory
         = Path.GetDirectoryName(typeof(JsonTestSqliteContext).Assembly.Location);

        private readonly string _connectionString;

        public JsonTestSqliteContext(string connectionString = "Data Source=file:json_extract.db")
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JsonEntityBasicString>().ToTable("JsonEntityBasicString");
        }

        public virtual DbSet<JsonEntityBasicString> JsonEntitiesBasicString { get; set; }
    }

}
