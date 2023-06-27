namespace JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.EntityTypeConfigurations
{
  using JamesPChadwick.Ingestion.Domain.Aggregates.FileAggregate;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;

  public class FileEntityTypeConfiguration : IEntityTypeConfiguration<File>
  {
    public void Configure(EntityTypeBuilder<File> builder)
    {
      builder.ToTable("File");

      builder.HasKey(b => b.Id);

      builder.Property(b => b.Id);

      builder.Property(b => b.Name)
        .HasMaxLength(256)
        .IsRequired();

      builder.Property(b => b.Hash)
        .HasMaxLength(64)
        .IsRequired();

      builder.Property(b => b.Path)
        .HasMaxLength(1024)
        .IsRequired();

      builder.Property(b => b.Size).IsRequired();

      builder.Property(b => b.CreatedOnUtc).IsRequired();

      builder.Ignore(b => b.Events);
    }
  }
}
