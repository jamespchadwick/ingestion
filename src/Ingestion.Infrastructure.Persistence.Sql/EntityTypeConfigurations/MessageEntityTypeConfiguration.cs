namespace JamesPChadwick.Ingestion.Infrastructure.Persistence.Sql.EntityTypeConfigurations
{
  using JamesPChadwick.Ingestion.Domain.Aggregates.MessageAggregate;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;

  public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
  {
    public void Configure(EntityTypeBuilder<Message> builder)
    {
      builder.ToTable("Message");

      builder.HasKey(b => b.Id);

      builder.Property(b => b.Id);

      builder.Property(b => b.Guid).IsRequired();

      builder.Property(b => b.Scope);

      builder
        .Property(b => b.Status)
        .HasColumnName("StatusId")
        .HasConversion(p => p.Value, p => MessageStatus.FromValue(p))
        .IsRequired();

      builder.Property(b => b.Type)
        .HasMaxLength(255)
        .IsRequired();

      builder.Property(b => b.Payload).IsRequired();

      builder.Property(b => b.CreatedOnUtc).IsRequired();

      builder.Property(b => b.TimesSent).IsRequired();

      builder.Ignore(b => b.Events);
    }
  }
}
