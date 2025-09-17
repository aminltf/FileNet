using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileNet.Infrastructure.Persistence.Configurations;

public class DocumentConfig : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(256);

        builder.Property(x => x.FileName)
            .HasMaxLength(260).IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(127).IsRequired();

        builder.Property(x => x.Size)
            .HasColumnType("bigint").IsRequired();

        builder.Property(x => x.Category)
            .HasConversion<byte>().IsRequired();

        builder.Property(x => x.Data)
            .HasColumnType("varbinary(max)").IsRequired();

        builder.HasOne(x => x.Employee)
             .WithMany(y => y.Documents)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => new { x.EmployeeId, x.Category });
    }
}
