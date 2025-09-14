using FileNet.WebFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FileNet.Tests;

internal class TestHelpers
{
    public static AppDbContext CreateInMemoryDb(string? name = null)
    {
        name ??= Guid.NewGuid().ToString("N");
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .EnableSensitiveDataLogging()
            .Options;
        var db = new AppDbContext(opts);
        db.Database.EnsureCreated();
        return db;
    }
}
