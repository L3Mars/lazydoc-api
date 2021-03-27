using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc
{
    public class LazyDocContext : DbContext
    {
        public LazyDocContext(DbContextOptions<LazyDocContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EntityDocument>()
                   .HasMany<EntityTag>(s => s.Tags)
                   .WithOne(g => g.Document)
                   .HasForeignKey(s => s.DocumentId);

            builder.Entity<EntityDocument>()
                   .HasMany<EntityFile>(s => s.Files)
                   .WithOne(g => g.Document)
                   .HasForeignKey(s => s.DocumentId);
        }

        public DbSet<EntityTag> Tags { get; set; }
        public DbSet<EntityDocument> Documents { get; set; }
        public DbSet<EntityFile> Files { get; set; }
        public DbSet<EntityUser> Users { get; set; }
    }
}
