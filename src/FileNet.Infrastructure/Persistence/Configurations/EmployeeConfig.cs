using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileNet.Infrastructure.Persistence.Configurations;

public class EmployeeConfig : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NationalCode)
            .HasMaxLength(20).IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100).IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100).IsRequired();

        builder.Property(x => x.Gender)
            .HasConversion<byte>().IsRequired();

        builder.HasOne(x => x.Department)
             .WithMany(y => y.Employees)
             .HasForeignKey(x => x.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.NationalCode).IsUnique();
    }
}
