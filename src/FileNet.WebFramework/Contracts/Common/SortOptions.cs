namespace FileNet.WebFramework.Contracts.Common;

public class SortOptions
{
    public List<string> Fields { get; init; } = new();
    public SortDirection Direction { get; init; } = SortDirection.Asc;
}

public enum SortDirection
{
    Asc = 0,
    Desc = 1
}
