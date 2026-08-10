// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlAuthExtractorTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var ctor = typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, Type.EmptyTypes, null);
        Assert.NotNull(ctor);
        return (HttpRequestBuilder)ctor.Invoke(null);
    }

    private static HttpCurlParsingContext CreateContext(params string[] tokens) => new(tokens);

    [Fact]
    public void FlagsWithArgument_ReturnOK() =>
        Assert.Equal(["-u", "--user", "--bearer"], CurlAuthExtractor._flagsWithArgument);

    [Fact]
    public void FlagsWithoutArgument_ReturnOK() =>
        Assert.Equal(["--basic", "--digest", "--any", "--negotiate", "--ntlm"],
            CurlAuthExtractor._flagsWithoutArgument);

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlAuthExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("curl", "http://example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.AuthenticationHeader);
    }

    [Fact]
    public void TryExtract_UserFlagWithArgument_ReturnOK()
    {
        var extractor = new CurlAuthExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-u", "admin:123456");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("admin:123456", decoded);
    }

    [Fact]
    public void TryExtract_BearerFlagWithArgument_ReturnOK()
    {
        var extractor = new CurlAuthExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--bearer", "my-token");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Bearer", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("my-token", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void TryExtract_FlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlAuthExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-u");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.AuthenticationHeader);
    }

    [Fact]
    public void TryExtract_SchemeFlagBasic_ReturnOK()
    {
        var extractor = new CurlAuthExtractor();
        var builder = CreateBuilder();
        builder.AddBasicAuthentication("user", "pass");
        var context = CreateContext("--basic");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("user:pass", decoded);
    }

    [Fact]
    public void TryExtract_SchemeFlagDigestFromBasic_ReturnOK()
    {
        var extractor = new CurlAuthExtractor();
        var builder = CreateBuilder();
        builder.AddBasicAuthentication("user", "pass");
        var context = CreateContext("--digest");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Digest", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("user|:|pass", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void TryExtract_SchemeFlagWhenNoPreviousAuth_ReturnOK()
    {
        var extractor = new CurlAuthExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--ntlm");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.AuthenticationHeader);
    }

    [Fact]
    public void ProcessAuthWithArgument_Bearer_ReturnOK()
    {
        var builder = CreateBuilder();
        CurlAuthExtractor.ProcessAuthWithArgument(builder, "--bearer", "abc123");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Bearer", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("abc123", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void ProcessAuthWithArgument_UserWithColon_ReturnOK()
    {
        var builder = CreateBuilder();
        CurlAuthExtractor.ProcessAuthWithArgument(builder, "-u", "user:pass");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("user:pass", decoded);
    }

    [Fact]
    public void ProcessAuthWithArgument_UserWithoutColon_ReturnOK()
    {
        var builder = CreateBuilder();
        CurlAuthExtractor.ProcessAuthWithArgument(builder, "--user", "username");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("username:", decoded);
    }

    [Fact]
    public void ProcessAuthScheme_NullCurrentAuth_ReturnOK()
    {
        var builder = CreateBuilder();
        CurlAuthExtractor.ProcessAuthScheme(builder, "--basic");
        Assert.Null(builder.AuthenticationHeader);
    }

    [Fact]
    public void ProcessAuthScheme_BasicToBasic_ReturnOK()
    {
        var builder = CreateBuilder();
        builder.AddBasicAuthentication("user", "pass");
        CurlAuthExtractor.ProcessAuthScheme(builder, "--basic");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("user:pass", decoded);
    }

    [Fact]
    public void ProcessAuthScheme_BasicToDigest_ReturnOK()
    {
        var builder = CreateBuilder();
        builder.AddBasicAuthentication("user", "pass");
        CurlAuthExtractor.ProcessAuthScheme(builder, "--digest");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Digest", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("user|:|pass", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void ProcessAuthScheme_BasicWithUnknownFlag_ReturnOK()
    {
        var builder = CreateBuilder();
        builder.AddBasicAuthentication("user", "pass");
        CurlAuthExtractor.ProcessAuthScheme(builder, "--ntlm");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("user:pass", decoded);
    }

    [Fact]
    public void ProcessAuthScheme_DigestToBasic_ReturnOK()
    {
        var builder = CreateBuilder();
        builder.AddDigestAuthentication("user", "pass");
        CurlAuthExtractor.ProcessAuthScheme(builder, "--basic");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Basic", builder.AuthenticationHeader!.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(builder.AuthenticationHeader.Parameter!));
        Assert.Equal("user:pass", decoded);
    }

    [Fact]
    public void ProcessAuthScheme_DigestToDigest_ReturnOK()
    {
        var builder = CreateBuilder();
        builder.AddDigestAuthentication("user", "pass");
        CurlAuthExtractor.ProcessAuthScheme(builder, "--digest");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Digest", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("user|:|pass", builder.AuthenticationHeader.Parameter);
    }

    [Fact]
    public void ProcessAuthScheme_DigestWithUnknownFlag_ReturnOK()
    {
        var builder = CreateBuilder();
        builder.AddDigestAuthentication("user", "pass");
        CurlAuthExtractor.ProcessAuthScheme(builder, "--any");

        Assert.NotNull(builder.AuthenticationHeader);
        Assert.Equal("Digest", builder.AuthenticationHeader!.Scheme);
        Assert.Equal("user|:|pass", builder.AuthenticationHeader.Parameter);
    }
}