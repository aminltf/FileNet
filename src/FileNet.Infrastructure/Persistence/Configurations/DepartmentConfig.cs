using FileNet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileNet.Infrastructure.Persistence.Configurations;

public class DepartmentConfig : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(31).IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(127).IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(511);

        builder.HasIndex(d => d.Code).IsUnique();
        builder.HasIndex(d => d.Name).IsUnique();
    }
}
