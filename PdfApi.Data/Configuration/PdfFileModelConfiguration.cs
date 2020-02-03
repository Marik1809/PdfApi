using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfApi.Model;

namespace PdfApi.Data.Configuration
{
    internal class PdfFileModelConfiguration : IEntityTypeConfiguration<PdfFileModel>
    {
        public void Configure(EntityTypeBuilder<PdfFileModel> builder)
        {
            builder.HasKey(m => m.Name);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200)
                .ValueGeneratedNever();

            builder.Property(m => m.Location)
                .IsRequired()
                .HasMaxLength(400);

            builder.Property(m => m.Size)
                .IsRequired();
        }
    }
}
