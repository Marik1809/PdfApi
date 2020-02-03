using Microsoft.EntityFrameworkCore;
using PdfApi.Data.Configuration;
using PdfApi.Model;

namespace PdfApi.Data
{
    public class PdfApiDbContext : DbContext
    {
        public DbSet<PdfFileModel> Files { get; set; }

        public PdfApiDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PdfFileModelConfiguration());
        }
    }
}
