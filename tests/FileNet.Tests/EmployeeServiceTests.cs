using FileNet.WebFramework.Contracts.Employees;
using FileNet.WebFramework.Services.Implementations;
using FluentAssertions;

namespace FileNet.Tests;

public class EmployeeServiceTests
{
    [Fact]
    public async Task Create_And_GetById_Works()
    {
        using var db = TestHelpers.CreateInMemoryDb();
        var svc = new EmployeeService(db);

        var dto = new EmployeeCreateDto { NationalCode = "1234567890", FirstName = "Ali", LastName = "Ahmadi" };
        var id = await svc.CreateAsync(dto, CancellationToken.None);

        var e = await svc.GetByIdAsync(id, CancellationToken.None);
        e.Should().NotBeNull();
        e!.NationalCode.Should().Be("1234567890");
        e.FirstName.Should().Be("Ali");
        e.LastName.Should().Be("Ahmadi");
    }

    [Fact]
    public async Task Create_Duplicate_NationalCode_Throws()
    {
        using var db = TestHelpers.CreateInMemoryDb();
        var svc = new EmployeeService(db);

        await svc.CreateAsync(new EmployeeCreateDto { NationalCode = "111", FirstName = "A", LastName = "B" }, CancellationToken.None);
        var act = () => svc.CreateAsync(new EmployeeCreateDto { NationalCode = "111", FirstName = "C", LastName = "D" }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Update_Changes_Fields()
    {
        using var db = TestHelpers.CreateInMemoryDb();
        var svc = new EmployeeService(db);

        var id = await svc.CreateAsync(new EmployeeCreateDto { NationalCode = "222", FirstName = "A", LastName = "B" }, CancellationToken.None);

        await svc.UpdateAsync(new EmployeeUpdateDto { NationalCode = "222", FirstName = "Ali", LastName = "Bagheri" }, CancellationToken.None);

        var e = await svc.GetByIdAsync(id, CancellationToken.None);
        e!.FirstName.Should().Be("Ali");
        e!.LastName.Should().Be("Bagheri");
    }
}
