using FileNet.Domain.Constants;
using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileNet.Infrastructure.Persistence.Configurations;

public class EmployeeConfig : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("Employees");

        b.HasKey(x => x.Id);

        b.Property(x => x.ConcurrencyToken).IsRowVersion();

        b.Property(x => x.DepartmentId).IsRequired();

        b.Property(x => x.NationalCode)
         .HasMaxLength(DomainModelConstraints.NationalCodeMaxLen)
         .IsRequired();

        b.Property(x => x.EmployeeCode)
         .HasMaxLength(DomainModelConstraints.EmployeeCodeMaxLen)
         .IsRequired();

        b.Property(x => x.FirstName)
         .HasMaxLength(DomainModelConstraints.FirstNameMaxLen)
         .IsRequired();

        b.Property(x => x.LastName)
         .HasMaxLength(DomainModelConstraints.LastNameMaxLen)
         .IsRequired();

        b.Property(x => x.Gender)
         .HasConversion<short>() // map enum to SMALLINT
         .IsRequired();

        b.Property(x => x.CreatedOn).IsRequired();
        b.Property(x => x.ModifiedOn);

        b.HasQueryFilter(x => !x.IsDeleted);

        // Optional: enforce FK in DB
        b.HasOne<Department>()
         .WithMany()
         .HasForeignKey(x => x.DepartmentId)
         .OnDelete(DeleteBehavior.Restrict);

        // Unique constraints with soft-delete filter
        b.HasIndex(x => x.NationalCode)
         .IsUnique()
         .HasFilter("[IsDeleted] = 0 AND [NationalCode] IS NOT NULL");

        b.HasIndex(x => x.EmployeeCode)
         .IsUnique()
         .HasFilter("[IsDeleted] = 0 AND [EmployeeCode] IS NOT NULL");
    }
}
