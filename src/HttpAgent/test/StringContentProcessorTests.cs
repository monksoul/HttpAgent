// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StringContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new StringContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(StringContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new StringContentProcessor();

        Assert.True(processor.CanProcess(new HttpContentProcessorContext(new StringContent(""), "text/plain")));
        Assert.True(processor.CanProcess(
            new HttpContentProcessorContext(JsonContent.Create(new { id = 1, name = "Furion" }), "application/json")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/json")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/json-patch+json")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/xml")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/xml-patch+xml")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "text/xml")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "text/html")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "text/plain")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "Text/Plain")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/soap+xml")));

        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "application/x-www-form-urlencoded")));
        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "application/octet-stream")));
        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "multipart/form-data")));
        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "image/jpeg")));
        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "image/png")));

        Assert.False(processor.CanProcess(new HttpContentProcessorContext(
            new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()),
            "application/x-www-form-urlencoded")));
    }

    [Fact]
    public async Task Process_ReturnOK()
    {
        var processor = new StringContentProcessor();

        Assert.Null(processor.Process(new HttpContentProcessorContext(null, "application/json")));

        var stringContent = new StringContent("Hello World");
        var httpContent1 = processor.Process(new HttpContentProcessorContext(stringContent, "application/json"));
        Assert.Same(stringContent, httpContent1);

        var httpContent2 = processor.Process(new HttpContentProcessorContext("furion", "text/plain"));
        Assert.NotNull(httpContent2);
        Assert.Equal(typeof(StringContent), httpContent2.GetType());
        Assert.Equal("furion", await httpContent2.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent2.Headers.ContentType?.MediaType);
        Assert.Null(httpContent2.Headers.ContentType?.CharSet);

        var httpContent3 = processor.Process(new HttpContentProcessorContext(new { id = 10, name = "furion" },
            "application/json", Encoding.UTF32));
        Assert.NotNull(httpContent3);
        Assert.Equal(typeof(StringContent), httpContent3.GetType());
        Assert.Equal("{\"id\":10,\"name\":\"furion\"}", await httpContent3.ReadAsStringAsync());
        Assert.Equal("application/json", httpContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", httpContent3.Headers.ContentType?.CharSet);

        var httpContent4 = processor.Process(new HttpContentProcessorContext(10, "text/plain"));
        Assert.NotNull(httpContent4);
        Assert.Equal(typeof(StringContent), httpContent4.GetType());
        Assert.Equal("10", await httpContent4.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent4.Headers.ContentType?.MediaType);
        Assert.Null(httpContent4.Headers.ContentType?.CharSet);

        var httpContent5 = processor.Process(new HttpContentProcessorContext(EnumModel.Default, "text/plain"));
        Assert.NotNull(httpContent5);
        Assert.Equal(typeof(StringContent), httpContent5.GetType());
        Assert.Equal("1", await httpContent5.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent5.Headers.ContentType?.MediaType);
        Assert.Null(httpContent5.Headers.ContentType?.CharSet);

        using var jsonDocument = JsonDocument.Parse("\"furion\"");
        var httpContent6 =
            processor.Process(new HttpContentProcessorContext(jsonDocument.RootElement, "application/json",
                Encoding.UTF32));
        Assert.NotNull(httpContent6);
        Assert.Equal(typeof(StringContent), httpContent6.GetType());
        Assert.Equal("furion", await httpContent6.ReadAsStringAsync());
        Assert.Equal("application/json", httpContent6.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", httpContent6.Headers.ContentType?.CharSet);

        var httpContent7 =
            processor.Process(new HttpContentProcessorContext(new StringContent("furion"), "text/plain"));
        Assert.NotNull(httpContent7);
        Assert.Equal(typeof(StringContent), httpContent7.GetType());
        Assert.Equal("furion", await httpContent7.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent7.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", httpContent7.Headers.ContentType?.CharSet);

        var services = new ServiceCollection();
        services.AddOptions<HttpRemoteOptions>();
        await using var serviceProvider = services.BuildServiceProvider();
        var processor2 = new StringContentProcessor { ServiceProvider = serviceProvider };
        var httpContent8 = processor2.Process(new HttpContentProcessorContext(new JsonModel { Id = 1, Name = "Furion" },
            "application/json"));
        Assert.NotNull(httpContent8);
        Assert.Equal(typeof(StringContent), httpContent8.GetType());
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", await httpContent8.ReadAsStringAsync());
        Assert.Equal("application/json", httpContent8.Headers.ContentType?.MediaType);
        Assert.Null(httpContent8.Headers.ContentType?.CharSet);
    }
}

file class JsonModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}