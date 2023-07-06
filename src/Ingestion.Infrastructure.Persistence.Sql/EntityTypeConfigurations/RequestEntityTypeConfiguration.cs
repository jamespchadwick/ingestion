namespace JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.EntityTypeConfigurations
{
  using JamesPChadwick.Ingestion.Domain.Aggregates.RequestAggregate;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;

  public class RequestEntityTypeConfiguration : IEntityTypeConfiguration<Request>
  {
    public void Configure(EntityTypeBuilder<Request> builder)
    {
      builder.ToTable("Request");

      builder.HasKey(b => b.Id);

      builder.Property(b => b.Id);

      builder.Property(b => b.IdempotencyKey)
        .HasMaxLength(255)
        .IsRequired();

      builder
        .Property(b => b.Status)
        .HasColumnName("StatusId")
        .HasConversion(p => p.Value, p => RequestStatus.FromValue(p))
        .IsRequired();

      builder.Property(b => b.Type)
        .HasMaxLength(255)
        .IsRequired();

      builder.Property(b => b.Payload).IsRequired();

      builder.Property(b => b.TimeStamp).IsRequired();

      builder.Property(b => b.TimesProcessed);

      builder.Property(b => b.IsSuccess);

      builder.Property(b => b.Message);

      builder.Property(b => b.LastProcessedOnUtc);
    }
  }
}