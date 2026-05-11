// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpContentProcessorFactoryTests
{
    [Fact]
    public void New_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentProcessorFactory1 = new HttpContentProcessorFactory(serviceProvider, null);
        Assert.NotNull(httpContentProcessorFactory1.ServiceProvider);
        Assert.NotNull(httpContentProcessorFactory1._processors);
        Assert.Equal(7, httpContentProcessorFactory1._processors.Count);
        Assert.Equal(
            [
                typeof(StringContentProcessor), typeof(FormUrlEncodedContentProcessor),
                typeof(ByteArrayContentProcessor), typeof(StreamContentProcessor),
                typeof(MultipartFormDataContentProcessor), typeof(ReadOnlyMemoryContentProcessor),
                typeof(JsonLinesContentProcessor)
            ],
            httpContentProcessorFactory1._processors.Select(u => u.Key));

        var httpContentProcessorFactory2 =
            new HttpContentProcessorFactory(serviceProvider, [new CustomStringContentProcessor()]);
        Assert.NotNull(httpContentProcessorFactory2._processors);
        Assert.Equal(8, httpContentProcessorFactory2._processors.Count);
        Assert.Equal(
            [
                typeof(StringContentProcessor), typeof(FormUrlEncodedContentProcessor),
                typeof(ByteArrayContentProcessor), typeof(StreamContentProcessor),
                typeof(MultipartFormDataContentProcessor), typeof(ReadOnlyMemoryContentProcessor),
                typeof(JsonLinesContentProcessor), typeof(CustomStringContentProcessor)
            ],
            httpContentProcessorFactory2._processors.Select(u => u.Key));

        var httpContentProcessorFactory3 =
            new HttpContentProcessorFactory(serviceProvider,
                [new StringContentProcessor(), new FormUrlEncodedContentProcessor()]);
        Assert.NotNull(httpContentProcessorFactory3._processors);
        Assert.Equal(7, httpContentProcessorFactory3._processors.Count);
        Assert.Equal(
            [
                typeof(StringContentProcessor), typeof(FormUrlEncodedContentProcessor),
                typeof(ByteArrayContentProcessor), typeof(StreamContentProcessor),
                typeof(MultipartFormDataContentProcessor), typeof(ReadOnlyMemoryContentProcessor),
                typeof(JsonLinesContentProcessor)
            ],
            httpContentProcessorFactory3._processors.Select(u => u.Key));
    }

    [Fact]
    public void GetProcessor_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var httpContentProcessorFactory1 = new HttpContentProcessorFactory(serviceProvider, null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            httpContentProcessorFactory1.GetProcessor(null, "image/jpeg");
        });

        Assert.Equal(
            "No processor found that can handle the content type `image/jpeg` and the provided raw content of type ``. Please ensure that the correct content type is specified and that a suitable processor is registered.",
            exception.Message);
    }

    [Theory]
    [InlineData("application/json", typeof(StringContentProcessor))]
    [InlineData("application/json-patch+json", typeof(StringContentProcessor))]
    [InlineData("application/xml", typeof(StringContentProcessor))]
    [InlineData("application/xml-patch+xml", typeof(StringContentProcessor))]
    [InlineData("text/xml", typeof(StringContentProcessor))]
    [InlineData("text/html", typeof(StringContentProcessor))]
    [InlineData("text/plain", typeof(StringContentProcessor))]
    [InlineData("application/x-www-form-urlencoded", typeof(FormUrlEncodedContentProcessor))]
    [InlineData("multipart/form-data", typeof(MultipartFormDataContentProcessor))]
    [InlineData("application/x-ndjson", typeof(JsonLinesContentProcessor))]
    [InlineData("application/x-jsonlines", typeof(JsonLinesContentProcessor))]
    [InlineData("application/jsonlines", typeof(JsonLinesContentProcessor))]
    [InlineData("application/jsonl", typeof(JsonLinesContentProcessor))]
    public void GetProcessor_From_ContentType_ReturnOK(string contentType, Type type)
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var httpContentProcessorFactory1 = new HttpContentProcessorFactory(serviceProvider, null);

        var processor1 = httpContentProcessorFactory1.GetProcessor(null, contentType);
        Assert.Equal(type, processor1.GetType());
    }

    [Fact]
    public void GetProcessor_From_RawContent_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var httpContentProcessorFactory1 = new HttpContentProcessorFactory(serviceProvider, null);

        var processor1 = httpContentProcessorFactory1.GetProcessor(new StringContent(""), "application/json");
        Assert.Equal(typeof(StringContentProcessor), processor1.GetType());

        var processor2 =
            httpContentProcessorFactory1.GetProcessor(new FormUrlEncodedContent([]),
                "application/x-www-form-urlencoded");
        Assert.Equal(typeof(FormUrlEncodedContentProcessor), processor2.GetType());

        var processor3 = httpContentProcessorFactory1.GetProcessor(new ByteArrayContent([]),
            "application/octet-stream");
        Assert.Equal(typeof(ByteArrayContentProcessor), processor3.GetType());

        using var stream = new MemoryStream();
        var processor4 = httpContentProcessorFactory1.GetProcessor(new StreamContent(stream),
            "application/octet-stream");
        Assert.Equal(typeof(StreamContentProcessor), processor4.GetType());

        var processor5 = httpContentProcessorFactory1.GetProcessor(Array.Empty<byte>(),
            "application/octet-stream");
        Assert.Equal(typeof(ByteArrayContentProcessor), processor5.GetType());

        var processor6 = httpContentProcessorFactory1.GetProcessor(stream,
            "application/octet-stream");
        Assert.Equal(typeof(StreamContentProcessor), processor6.GetType());

        var processor7 = httpContentProcessorFactory1.GetProcessor(new MultipartContent(),
            "multipart/form-data");
        Assert.Equal(typeof(MultipartFormDataContentProcessor), processor7.GetType());

        var processor8 = httpContentProcessorFactory1.GetProcessor(new { }, "application/json");
        Assert.Equal(typeof(StringContentProcessor), processor8.GetType());

        var processor9 =
            httpContentProcessorFactory1.GetProcessor(new ReadOnlyMemory<byte>([]),
                "application/octet-stream");
        Assert.Equal(typeof(ReadOnlyMemoryContentProcessor), processor9.GetType());

        var processor10 =
            httpContentProcessorFactory1.GetProcessor(new ReadOnlyMemoryContent(new ReadOnlyMemory<byte>([])),
                "application/octet-stream");
        Assert.Equal(typeof(ReadOnlyMemoryContentProcessor), processor10.GetType());
    }

    [Fact]
    public void GetProcessor_WithCustomize_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var httpContentProcessorFactory1 =
            new HttpContentProcessorFactory(serviceProvider, [new CustomStringContentProcessor()]);
        var processor1 = httpContentProcessorFactory1.GetProcessor(new StringContent(""), "application/json");
        Assert.Equal(typeof(CustomStringContentProcessor), processor1.GetType());

        var processor2 = httpContentProcessorFactory1.GetProcessor(new StringContent(""), "application/json",
            new CustomStringContentProcessor2());
        Assert.Equal(typeof(CustomStringContentProcessor2), processor2.GetType());
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var httpContentProcessorFactory1 = new HttpContentProcessorFactory(serviceProvider, null);

        var httpContent1 = httpContentProcessorFactory1.Build("", "application/json");
        Assert.NotNull(httpContent1);
        Assert.Equal(typeof(StringContent), httpContent1.GetType());

        var httpContent2 =
            httpContentProcessorFactory1.Build(new Dictionary<string, string>(),
                "application/x-www-form-urlencoded");
        Assert.NotNull(httpContent2);
        Assert.Equal(typeof(FormUrlEncodedContent), httpContent2.GetType());

        var httpContent3 =
            httpContentProcessorFactory1.Build(Array.Empty<byte>(), "application/octet-stream");
        Assert.NotNull(httpContent3);
        Assert.Equal(typeof(ByteArrayContent), httpContent3.GetType());

        using var stream = new MemoryStream();
        var httpContent4 = httpContentProcessorFactory1.Build(stream, "application/octet-stream");
        Assert.NotNull(httpContent4);
        Assert.Equal(typeof(StreamContent), httpContent4.GetType());
    }
}