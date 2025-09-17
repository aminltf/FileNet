namespace FileNet.Application.Extensions;

public static class FluentValidationExtensions
{
    public static bool IsValidIranianNationalCode(this string nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode) ||
            nationalCode.Length != 10 ||
            nationalCode.All(c => c == '0') ||
            !nationalCode.All(char.IsDigit))
            return false;

        var check = Convert.ToInt32(nationalCode[9].ToString());
        var sum = Enumerable.Range(0, 9)
            .Select(i => Convert.ToInt32(nationalCode[i].ToString()) * (10 - i))
            .Sum() % 11;

        return (sum < 2 && check == sum) || (sum >= 2 && check == (11 - sum));
    }
}
