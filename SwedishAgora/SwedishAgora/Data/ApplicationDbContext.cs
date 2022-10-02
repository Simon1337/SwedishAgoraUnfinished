using Microsoft.EntityFrameworkCore;
using SwedishAgora.DataLayer.DatabaseModels;

namespace SwedishAgora.Data
{
    public sealed class ApplicationDbContext : DbContext
    {
        public DbSet<User>? Users { get; set; }
        public DbSet<ProductTag>? ProductTags { get; set; }
        public DbSet<ProductCategory>? ProductCategory { get; set; }
        public DbSet<ProductListItems>? ProductListItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }
    }
}