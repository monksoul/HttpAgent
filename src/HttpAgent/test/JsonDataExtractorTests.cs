// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonDataExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonDataExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("data", prop!.GetValue(new JsonDataExtractor()));
    }

    [Fact]
    public void Extract_WithContentType_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"data\":{\"name\":\"test\"},\"contentType\":\"application/json\"}");
        new JsonDataExtractor().Extract(builder, context);
        Assert.NotNull(builder.RawContent);
        Assert.Equal("application/json", builder.ContentType);
    }

    [Fact]
    public void Extract_WithoutContentType_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"data\":\"hello\"}");
        new JsonDataExtractor().Extract(builder, context);
        Assert.NotNull(builder.RawContent);
        Assert.Null(builder.ContentType);
    }

    [Fact]
    public void Extract_WithEncoding_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"data\":\"text\",\"encoding\":\"utf-8\"}");
        new JsonDataExtractor().Extract(builder, context);
        Assert.NotNull(builder.ContentEncoding);
        Assert.Equal(Encoding.UTF8, builder.ContentEncoding);
    }

    [Fact]
    public void Extract_NoDataProperty_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{}");
        new JsonDataExtractor().Extract(builder, context);
        Assert.Null(builder.RawContent);
    }

    [Fact]
    public void Extract_FormUrlEncodedContent_ReturnOK()
    {
        var builder = CreateBuilder();
        var context =
            CreateContext("{\"data\":{\"key\":\"value\"},\"contentType\":\"application/x-www-form-urlencoded\"}");
        new JsonDataExtractor().Extract(builder, context);
        Assert.NotNull(builder.RawContent);
        Assert.Equal("application/x-www-form-urlencoded", builder.ContentType);
        Assert.NotNull(builder.HttpContentProcessorProviders);
        Assert.NotEmpty(builder.HttpContentProcessorProviders);
    }

    [Fact]
    public void Extract_WithEncodingInvalid_Invalid_Parameters()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"data\":\"text\",\"encoding\":\"invalid-encoding\"}");
        Assert.Throws<ArgumentException>(() => new JsonDataExtractor().Extract(builder, context));
    }

    [Fact]
    public void Extract_WithCharsetInContentType_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"data\":\"text\",\"contentType\":\"text/plain; charset=utf-8\"}");
        new JsonDataExtractor().Extract(builder, context);
        Assert.NotNull(builder.RawContent);
        Assert.Equal("text/plain", builder.ContentType);
        Assert.NotNull(builder.ContentEncoding);
        Assert.Equal(Encoding.UTF8, builder.ContentEncoding);
    }
}