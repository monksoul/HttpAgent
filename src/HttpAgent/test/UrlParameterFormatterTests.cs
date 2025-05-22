// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class UrlParameterFormatterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var formatter = new UrlParameterFormatter();
        Assert.NotNull(formatter);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    [InlineData("Furion", "Furion")]
    public void Format_ReturnOK(object? value, string? result)
    {
        var formatter = new UrlParameterFormatter();
        Assert.Equal(result, formatter.Format(value, new UrlFormattingContext()));
    }

    [Fact]
    public void Format_DateTime_ReturnOK()
    {
        var formatter = new UrlParameterFormatter();
        Assert.Equal("2024-08-20T20:21:00.0000000Z",
            formatter.Format(new DateTime(2024, 8, 20, 20, 21, 0, 0, DateTimeKind.Utc), new UrlFormattingContext()));

        var formatter2 = new TestUrlParameterFormatter();
        Assert.Equal("2024-08-20",
            formatter2.Format(new DateTime(2024, 8, 20, 20, 21, 0, 0, DateTimeKind.Utc), new UrlFormattingContext()));
    }
}

public class TestUrlParameterFormatter : UrlParameterFormatter
{
    /// <inheritdoc />
    public override string? Format(object? value, UrlFormattingContext context)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        return base.Format(value, context);
    }
}