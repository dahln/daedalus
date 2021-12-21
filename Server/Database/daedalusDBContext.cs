using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace climatepi.Server.Database
{
    public class climatepiDBContext : DbContext
    {
        public climatepiDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Condition> Conditions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
