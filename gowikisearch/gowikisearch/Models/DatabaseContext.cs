using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace gowikisearch.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {

        }

        public DbSet<WikipediaPageTitle> WikipediaPageTitles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }
    }
}