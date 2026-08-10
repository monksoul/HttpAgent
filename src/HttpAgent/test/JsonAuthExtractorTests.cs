// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonAuthExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonAuthExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("auth", prop!.GetValue(new JsonAuthExtractor()));
    }

    [Fact]
    public void Aliases_ReturnOK()
    {
        var prop = typeof(JsonAuthExtractor).GetProperty("Aliases", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(new[] { "authentication", "authorization" }, prop!.GetValue(new JsonAuthExtractor()));
    }

    [Fact]
    public void Extract_BearerWithDefaultHeader_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"auth\":{\"type\":\"bearer\",\"token\":\"abc123\"}}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Bearer", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("abc123", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void Extract_BearerWithCustomHeader_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"auth\":{\"type\":\"bearer\",\"token\":\"tok\",\"header\":\"X-Auth\"}}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.NotNull(builder.Headers);
        Assert.True(builder.Headers!.ContainsKey("X-Auth"));
        Assert.Equal("Bearer tok", builder.Headers["X-Auth"][0]);
    }

    [Fact]
    public void Extract_Basic_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"auth\":{\"type\":\"basic\",\"username\":\"admin\",\"password\":\"pass\"}}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("admin:pass", decoded);
    }

    [Fact]
    public void Extract_BasicWithoutPassword_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"auth\":{\"type\":\"basic\",\"username\":\"admin\"}}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.NotNull(builder.AuthenticationHeader);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader!.Parameter!));
        Assert.Equal("admin:", decoded);
    }

    [Fact]
    public void Extract_Digest_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"auth\":{\"type\":\"digest\",\"username\":\"user\",\"password\":\"secret\"}}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Digest", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("user|:|secret", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void Extract_InvalidType_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"auth\":{\"type\":\"unknown\"}}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.Null(builder.AuthenticationHeader);
    }

    [Fact]
    public void Extract_NoAuthProperty_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.Null(builder.AuthenticationHeader);
    }

    [Fact]
    public void Extract_Alias_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"authentication\":{\"type\":\"bearer\",\"token\":\"tok\"}}");
        new JsonAuthExtractor().Extract(builder, context);
        Assert.NotNull(builder.AuthenticationHeader);
    }
}