using FileNet.Domain.Constants;
using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileNet.Infrastructure.Persistence.Configurations;

public class DepartmentConfig : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> b)
    {
        b.ToTable("Departments");

        b.HasKey(x => x.Id);

        // Concurrency
        b.Property(x => x.ConcurrencyToken)
         .IsRowVersion(); // SQL Server. For other providers: IsConcurrencyToken() + ValueGeneratedOnAddOrUpdate()

        // Requireds & lengths
        b.Property(x => x.Code)
         .HasMaxLength(DomainModelConstraints.DepartmentCodeMaxLen)
         .IsRequired();

        b.Property(x => x.Name)
         .HasMaxLength(DomainModelConstraints.DepartmentNameMaxLen)
         .IsRequired();

        b.Property(x => x.Description)
         .HasMaxLength(DomainModelConstraints.DepartmentDescriptionMaxLen);

        // Audit timestamps
        b.Property(x => x.CreatedOn).IsRequired();
        b.Property(x => x.ModifiedOn);

        // SoftDelete global query filter
        b.HasQueryFilter(x => !x.IsDeleted);

        // Filtered unique index
        b.HasIndex(x => x.Code)
         .IsUnique()
         .HasFilter("[IsDeleted] = 0 AND [Code] IS NOT NULL");
    }
}
