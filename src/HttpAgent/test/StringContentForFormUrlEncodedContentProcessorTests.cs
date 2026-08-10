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
        Assert.True(processor.UrlEncode);
        Assert.NotNull(processor);
        Assert.True(
            typeof(IHttpContentProcessor).IsAssignableFrom(typeof(StringContentForFormUrlEncodedContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var formUrlEncodedContentProcessor = new StringContentForFormUrlEncodedContentProcessor();

        Assert.False(
            formUrlEncodedContentProcessor.CanProcess(
                new HttpContentProcessorContext(null, "application/octet-stream")));
        Assert.True(
            formUrlEncodedContentProcessor.CanProcess(
                new HttpContentProcessorContext(null, "application/x-www-form-urlencoded")));
        Assert.True(
            formUrlEncodedContentProcessor.CanProcess(
                new HttpContentProcessorContext(null, "Application/X-www-form-urlencoded")));
        Assert.False(
            formUrlEncodedContentProcessor.CanProcess(new HttpContentProcessorContext(null, "application/json")));
        Assert.True(
            formUrlEncodedContentProcessor.CanProcess(
                new HttpContentProcessorContext(new { }, "application/x-www-form-urlencoded")));
        Assert.True(formUrlEncodedContentProcessor.CanProcess(
            new HttpContentProcessorContext(new FormUrlEncodedContent([]), "application/x-www-form-urlencoded")));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new StringContentForFormUrlEncodedContentProcessor();

        Assert.Throws<NotSupportedException>(() =>
        {
            processor.Process(new HttpContentProcessorContext(1, "application/x-www-form-urlencoded"));
        });
    }

    [Fact]
    public async Task Process_ReturnOK()
    {
        var processor = new StringContentForFormUrlEncodedContentProcessor();

        Assert.Null(
            processor.Process(new HttpContentProcessorContext(null, "application/x-www-form-urlencoded")));

        var formUrlEncodedContent =
            new FormUrlEncodedContent(new List<KeyValuePair<string, string>> { new("key", "value") });
        var httpContent1 =
            processor.Process(new HttpContentProcessorContext(formUrlEncodedContent,
                "application/x-www-form-urlencoded"));
        Assert.Same(formUrlEncodedContent, httpContent1);

        var httpContent2 =
            processor.Process(new HttpContentProcessorContext(new { }, "application/x-www-form-urlencoded"));
        Assert.NotNull(httpContent2);
        Assert.Equal(typeof(StringContent), httpContent2.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent2.Headers.ContentType?.MediaType);
        Assert.Null(httpContent2.Headers.ContentType?.CharSet);

        var httpContent3 = processor.Process(new HttpContentProcessorContext(new { id = 1, name = "furion" },
            "application/x-www-form-urlencoded", Encoding.UTF32));
        Assert.NotNull(httpContent3);
        Assert.Equal(typeof(StringContent), httpContent3.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", httpContent3.Headers.ContentType?.CharSet);

        var result = await httpContent3.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal("id=1&name=furion", result);

        var httpContent4 = processor.Process(new HttpContentProcessorContext(
            new StringContent("id=1&name=furion", null, "application/x-www-form-urlencoded"),
            "application/x-www-form-urlencoded"));
        Assert.NotNull(httpContent4);
        Assert.Equal(typeof(StringContent), httpContent4.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent4.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", httpContent4.Headers.ContentType?.CharSet);

        var processor2 = new StringContentForFormUrlEncodedContentProcessor { UrlEncode = false };
        var httpContent5 =
            processor2.Process(new HttpContentProcessorContext(
                new KeyValuePair<string, string?>[]
                {
                    new("id", "1"), new("name", "fur ion"), new("data", """{"plateNo":"京A12345","color":"1"}""")
                }, "application/x-www-form-urlencoded"))!;
        Assert.Equal("""id=1&name=fur ion&data={"plateNo":"京A12345","color":"1"}""",
            await httpContent5.ReadAsStringAsync(TestContext.Current.CancellationToken));

        var processor3 = new StringContentForFormUrlEncodedContentProcessor { UrlEncode = true };
        var httpContent6 =
            processor3.Process(new HttpContentProcessorContext(
                "id=1&name=fur ion&data={\"plateNo\":\"京A12345\",\"color\":\"1\"}",
                "application/x-www-form-urlencoded"))!;
        Assert.Equal("id=1&name=fur+ion&data=%7B%22plateNo%22%3A%22%E4%BA%ACA12345%22%2C%22color%22%3A%221%22%7D",
            await httpContent6.ReadAsStringAsync(TestContext.Current.CancellationToken));

        var httpContent7 =
            processor.Process(new HttpContentProcessorContext("id=1&name=furion", "application/x-www-form-urlencoded"));
        Assert.NotNull(httpContent7);
        Assert.Equal(typeof(StringContent), httpContent7.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent7.Headers.ContentType?.MediaType);
        Assert.Null(httpContent7.Headers.ContentType?.CharSet);

        var httpContent8 =
            processor.Process(new HttpContentProcessorContext(JsonNode.Parse("\"id=1&name=furion\""),
                "application/x-www-form-urlencoded"));
        Assert.NotNull(httpContent8);
        Assert.Equal(typeof(StringContent), httpContent8.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent8.Headers.ContentType?.MediaType);
        Assert.Null(httpContent8.Headers.ContentType?.CharSet);

#if NET10_0_OR_GREATER
        var httpContent9 =
            processor.Process(new HttpContentProcessorContext(JsonElement.Parse("\"id=1&name=furion\""),
                "application/x-www-form-urlencoded"));
        Assert.NotNull(httpContent9);
        Assert.Equal(typeof(StringContent), httpContent9.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent9.Headers.ContentType?.MediaType);
        Assert.Null(httpContent9.Headers.ContentType?.CharSet);
#endif
    }

    [Fact]
    public void GetContentString_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() =>
        StringContentForFormUrlEncodedContentProcessor.GetContentString(null!));

    [Fact]
    public void GetContentString_ReturnOK()
    {
        var result =
            StringContentForFormUrlEncodedContentProcessor.GetContentString(
                [new KeyValuePair<string, string?>("id", "1"), new KeyValuePair<string, string?>("name", "furion")]);
        Assert.Equal("id=1&name=furion", result);

        var result2 =
            StringContentForFormUrlEncodedContentProcessor.GetContentString(
                [new KeyValuePair<string, string?>("id", "1"), new KeyValuePair<string, string?>("name", "fur ion")]);
        Assert.Equal("id=1&name=fur+ion", result2);

        var result3 =
            StringContentForFormUrlEncodedContentProcessor.GetContentString(
            [
                new KeyValuePair<string, string?>("id", "1"), new KeyValuePair<string, string?>("name", "fur ion"),
                new KeyValuePair<string, string?>("data", """{"plateNo":"京A12345","color":"1"}""")
            ], false);
        Assert.Equal("""id=1&name=fur ion&data={"plateNo":"京A12345","color":"1"}""", result3);
    }

    [Fact]
    public void Encode_ReturnOK()
    {
        Assert.Equal(string.Empty, StringContentForFormUrlEncodedContentProcessor.Encode(null));
        Assert.Equal(string.Empty, StringContentForFormUrlEncodedContentProcessor.Encode(string.Empty));
        Assert.Equal("+", StringContentForFormUrlEncodedContentProcessor.Encode(" "));
    }
}