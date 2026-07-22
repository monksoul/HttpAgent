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
        Assert.Equal(result,
            formatter.Format(new UrlFormattingContext("name", null), "value", [value])?.FirstOrDefault().Value);
    }

    [Fact]
    public void Format_DateTime_ReturnOK()
    {
        var formatter = new UrlParameterFormatter();
        Assert.Equal("2024-08-20T20:21:00.0000000Z",
            formatter.Format(new UrlFormattingContext("name", null), "date",
                [new DateTime(2024, 8, 20, 20, 21, 0, 0, DateTimeKind.Utc)])?.FirstOrDefault().Value);

        var formatter2 = new TestUrlParameterFormatter();
        Assert.Equal("2024-08-20",
            formatter2.Format(new UrlFormattingContext("name", null), "date",
                [new DateTime(2024, 8, 20, 20, 21, 0, 0, DateTimeKind.Utc)])?.FirstOrDefault().Value);
    }

    [Fact]
    public void Format_ValueProvider_ReturnOK()
    {
        var formatter = new UrlParameterFormatter();
        Assert.Equal("Furion",
            formatter.Format(new UrlFormattingContext("name", null), "value", [() => "Furion"])?.FirstOrDefault()
                .Value);
        Assert.Equal("Furion_Key",
            formatter.Format(new UrlFormattingContext("Key", null), "value",
                [(UrlFormattingContext context) => "Furion_" + context.Key])?.FirstOrDefault().Value);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    [InlineData("Furion", "Furion")]
    public void FormatValue_ReturnOK(object? value, string? result) => Assert.Equal(result,
        UrlParameterFormatter.FormatValue(new UrlFormattingContext("name", null), value));

    [Fact]
    public void FormatValue_DateTime_ReturnOK() =>
        Assert.Equal("2024-08-20T20:21:00.0000000Z",
            UrlParameterFormatter.FormatValue(new UrlFormattingContext("name", null),
                new DateTime(2024, 8, 20, 20, 21, 0, 0, DateTimeKind.Utc)));

    [Fact]
    public void FormatValue_ValueProvider_ReturnOK()
    {
        Assert.Equal("Furion",
            UrlParameterFormatter.FormatValue(new UrlFormattingContext("name", null), () => "Furion"));
        Assert.Equal("Furion_Key",
            UrlParameterFormatter.FormatValue(new UrlFormattingContext("Key", null),
                (UrlFormattingContext context) => "Furion_" + context.Key));
    }
}

public class TestUrlParameterFormatter : UrlParameterFormatter
{
    /// <inheritdoc />
    public override IEnumerable<KeyValuePair<string, string?>>? Format(UrlFormattingContext context, string key,
        IEnumerable<object?> values)
    {
        foreach (var value in values)
        {
            if (value is DateTime dateTime)
            {
                yield return new KeyValuePair<string, string?>(key, dateTime.ToString("yyyy-MM-dd"));
            }
            else
            {
                yield return new KeyValuePair<string, string?>(key, FormatValue(context, value));
            }
        }
    }
}