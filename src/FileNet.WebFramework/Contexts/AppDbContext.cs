using FileNet.WebFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileNet.WebFramework.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        // Employee
        model.Entity<Employee>(b =>
        {
            b.ToTable("Employees");
            b.HasKey(e => e.Id);

            b.Property(e => e.NationalCode).HasMaxLength(20).IsRequired();
            b.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            b.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            b.Property(d => d.Gender).HasConversion<byte>().IsRequired();

            b.HasOne(e => e.Department)
             .WithMany(d => d.Employees)
             .HasForeignKey(e => e.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(e => e.NationalCode).IsUnique();
        });

        // Document
        model.Entity<Document>(b =>
        {
            b.ToTable("Documents");
            b.HasKey(d => d.Id);

            b.Property(d => d.Title).HasMaxLength(256);
            b.Property(d => d.FileName).HasMaxLength(260).IsRequired();
            b.Property(d => d.ContentType).HasMaxLength(127).IsRequired();
            b.Property(d => d.Size).HasColumnType("bigint").IsRequired();

            b.Property(d => d.Category).HasConversion<byte>().IsRequired();
            b.Property(d => d.Data).HasColumnType("varbinary(max)").IsRequired();

            b.HasOne(d => d.Employee)
             .WithMany(e => e.Documents)
             .HasForeignKey(d => d.EmployeeId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(d => d.EmployeeId);
            b.HasIndex(d => new { d.EmployeeId, d.Category });
        });

        // Department
        model.Entity<Department>(b =>
        {
            b.ToTable("Departments");
            b.HasKey(d => d.Id);

            b.Property(d => d.Code).HasMaxLength(31).IsRequired();
            b.Property(d => d.Name).HasMaxLength(127).IsRequired();
            b.Property(d => d.Description).HasMaxLength(511);

            b.HasIndex(d => d.Code).IsUnique();
            b.HasIndex(d => d.Name).IsUnique();
        });
    }
}
