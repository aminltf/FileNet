using FileNet.WebFramework.Enums;
using FileNet.WebFramework.ScanIngest;
using FluentAssertions;

namespace FileNet.Tests;

public class ScanFileNameParserTests
{
    private readonly ScanFileNameParser _parser = new();

    [Theory]
    [InlineData("1234567890__CONTRACT__20250914093025__Job_Agreement.pdf", "1234567890", DocumentCategory.Contract, "Job Agreement", ".pdf")]
    [InlineData("987654321__ID__20250914101500.tiff", "987654321", DocumentCategory.Identity, null, ".tiff")]
    [InlineData("1122334455__EDU__20250101080000__IELTS_Score.jpg", "1122334455", DocumentCategory.Education, "IELTS Score", ".jpg")]
    public void Parse_Valid_Names(string fileName, string nc, DocumentCategory cat, string? title, string ext)
    {
        var ok = _parser.TryParse(fileName, out var info, out var error);

        ok.Should().BeTrue(error);
        info!.NationalCode.Should().Be(nc);
        info!.Category.Should().Be(cat);
        info!.Title.Should().Be(title);
        info!.Extension.Should().Be(ext);
        info!.TimestampUtc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Parse_Maps_Category_Aliases()
    {
        _parser.TryParse("12345__CTR__20250101010101__X.pdf", out var a, out _).Should().BeTrue();
        a!.Category.Should().Be(DocumentCategory.Contract);

        _parser.TryParse("12345__IDENTITY__20250101010101__X.pdf", out var b, out _).Should().BeTrue();
        b!.Category.Should().Be(DocumentCategory.Identity);

        _parser.TryParse("12345__CERT__20250101010101__X.pdf", out var c, out _).Should().BeTrue();
        c!.Category.Should().Be(DocumentCategory.Education);
    }

    [Fact]
    public void Parse_Unknown_Category_FallsBack_To_Other()
    {
        _parser.TryParse("12345__SOMETHING__20250101010101__X.pdf", out var info, out _).Should().BeTrue();
        info!.Category.Should().Be(DocumentCategory.Other);
    }

    [Fact]
    public void Parse_Invalid_Timestamp_Fails()
    {
        _parser.TryParse("12345__ID__20250101__X.pdf", out var _, out var err).Should().BeFalse();
        err.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("badname.pdf")]
    [InlineData("123__ID__20250101010101__X.pdf")] // NC too short
    public void Parse_Invalid_Pattern_Fails(string name)
    {
        _parser.TryParse(name, out var _, out var err).Should().BeFalse();
        err.Should().NotBeNullOrWhiteSpace();
    }
}
