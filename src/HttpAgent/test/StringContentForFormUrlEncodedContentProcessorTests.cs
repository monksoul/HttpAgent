// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StringContentForFormUrlEncodedContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new StringContentForFormUrlEncodedContentProcessor();
        Assert.NotNull(processor);
        Assert.True(
            typeof(IHttpContentProcessor).IsAssignableFrom(typeof(StringContentForFormUrlEncodedContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var formUrlEncodedContentProcessor = new StringContentForFormUrlEncodedContentProcessor();

        Assert.False(formUrlEncodedContentProcessor.CanProcess(null, "application/octet-stream"));
        Assert.True(formUrlEncodedContentProcessor.CanProcess(null, "application/x-www-form-urlencoded"));
        Assert.True(formUrlEncodedContentProcessor.CanProcess(null, "Application/X-www-form-urlencoded"));
        Assert.False(formUrlEncodedContentProcessor.CanProcess(null, "application/json"));
        Assert.True(formUrlEncodedContentProcessor.CanProcess(new { }, "application/x-www-form-urlencoded"));
        Assert.True(formUrlEncodedContentProcessor.CanProcess(new FormUrlEncodedContent([]),
            "application/x-www-form-urlencoded"));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new StringContentForFormUrlEncodedContentProcessor();

        Assert.Throws<NotSupportedException>(() =>
        {
            processor.Process(1, "application/x-www-form-urlencoded", null);
        });

        var exception = Assert.Throws<FormatException>(() =>
        {
            processor.Process("furion", "application/x-www-form-urlencoded", null);
        });
        Assert.Equal("The content must contain only form url encoded string.", exception.Message);
    }

    [Fact]
    public async Task Process_ReturnOK()
    {
        var processor = new StringContentForFormUrlEncodedContentProcessor();

        Assert.Null(processor.Process(null, "application/x-www-form-urlencoded", null));

        var formUrlEncodedContent =
            new FormUrlEncodedContent(new List<KeyValuePair<string, string>> { new("key", "value") });
        var httpContent1 = processor.Process(formUrlEncodedContent, "application/x-www-form-urlencoded", null);
        Assert.Same(formUrlEncodedContent, httpContent1);

        var httpContent2 = processor.Process(new { }, "application/x-www-form-urlencoded", null);
        Assert.NotNull(httpContent2);
        Assert.Equal(typeof(StringContent), httpContent2.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent2.Headers.ContentType?.MediaType);
        Assert.Null(httpContent2.Headers.ContentType?.CharSet);

        var httpContent3 = processor.Process(new { id = 1, name = "furion" }, "application/x-www-form-urlencoded",
            Encoding.UTF32);
        Assert.NotNull(httpContent3);
        Assert.Equal(typeof(StringContent), httpContent3.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", httpContent3.Headers.ContentType?.CharSet);

        var result = await httpContent3.ReadAsStringAsync();
        Assert.Equal("id=1&name=furion", result);

        var httpContent4 = processor.Process(
            new StringContent("id=1&name=furion", null, "application/x-www-form-urlencoded"),
            "application/x-www-form-urlencoded",
            null);
        Assert.NotNull(httpContent4);
        Assert.Equal(typeof(StringContent), httpContent4.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent4.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", httpContent4.Headers.ContentType?.CharSet);
    }

    [Fact]
    public void GetContentString_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() =>
        StringContentForFormUrlEncodedContentProcessor.GetContentString(null!));

    [Fact]
    public void GetContentString_ReturnOK()
    {
        var result =
            StringContentForFormUrlEncodedContentProcessor.GetContentString(
                new KeyValuePair<string, string?>("id", "1"), new KeyValuePair<string, string?>("name", "furion"));
        Assert.Equal("id=1&name=furion", result);

        var result2 =
            StringContentForFormUrlEncodedContentProcessor.GetContentString(
                new KeyValuePair<string, string?>("id", "1"), new KeyValuePair<string, string?>("name", "fur ion"));
        Assert.Equal("id=1&name=fur+ion", result2);
    }

    [Fact]
    public void Encode_ReturnOK()
    {
        Assert.Equal(string.Empty, StringContentForFormUrlEncodedContentProcessor.Encode(null));
        Assert.Equal(string.Empty, StringContentForFormUrlEncodedContentProcessor.Encode(string.Empty));
        Assert.Equal("+", StringContentForFormUrlEncodedContentProcessor.Encode(" "));
    }
}