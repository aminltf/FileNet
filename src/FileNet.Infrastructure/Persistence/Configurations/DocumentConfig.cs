using FileNet.Domain.Constants;
using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileNet.Infrastructure.Persistence.Configurations;

public class DocumentConfig : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> b)
    {
        b.ToTable("Documents");

        b.HasKey(x => x.Id);

        b.Property(x => x.ConcurrencyToken).IsRowVersion();

        b.Property(x => x.EmployeeId).IsRequired();

        b.Property(x => x.Title)
         .HasMaxLength(DomainModelConstraints.DocumentTitleMaxLen)
         .IsRequired();

        b.Property(x => x.FileName)
         .HasMaxLength(DomainModelConstraints.FileNameMaxLen)
         .IsRequired();

        b.Property(x => x.ContentType)
         .HasMaxLength(DomainModelConstraints.ContentTypeMaxLen)
         .IsRequired();

        b.Property(x => x.SizeBytes).IsRequired();

        b.Property(x => x.Category)
         .HasConversion<short>() // map enum to SMALLINT
         .IsRequired();

        // store file payload in DB (varbinary(max))
        b.Property(x => x.Data)
         .HasColumnType("varbinary(max)")
         .IsRequired();

        b.Property(x => x.CreatedOn).IsRequired();
        b.Property(x => x.ModifiedOn);

        b.HasQueryFilter(x => !x.IsDeleted);

        // Relation to Employee (restrict delete across aggregates)
        b.HasOne<Employee>()
         .WithMany()
         .HasForeignKey(x => x.EmployeeId)
         .OnDelete(DeleteBehavior.Restrict);

        // Avoid duplicate file names per employee when active
        b.HasIndex(x => new { x.EmployeeId, x.FileName })
         .IsUnique()
         .HasFilter("[IsDeleted] = 0 AND [FileName] IS NOT NULL");
    }
}
