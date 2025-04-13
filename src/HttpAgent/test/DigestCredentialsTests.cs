// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class DigestCredentialsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var credentials = new DigestCredentials();
        Assert.Null(credentials.Username);
        Assert.Null(credentials.Password);
        Assert.Null(credentials.Realm);
        Assert.Null(credentials.Nonce);
        Assert.Null(credentials.Qop);
        Assert.Equal(0, credentials.Nc);
        Assert.Null(credentials.CNonce);
        Assert.Null(credentials.Opaque);
    }

    [Fact]
    public void GenerateMd5Hash_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => DigestCredentials.GenerateMd5Hash(null!));

    [Fact]
    public void GenerateMd5Hash_ReturnOK() =>
        Assert.Equal("bda66540c463dfdbe70666019f89e554", DigestCredentials.GenerateMd5Hash("furion"));

    [Fact]
    public void ExtractParameterValueFromHeader_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => DigestCredentials.ExtractParameterValueFromHeader(null!, null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.ExtractParameterValueFromHeader(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.ExtractParameterValueFromHeader(" ", null!));

        Assert.Throws<ArgumentNullException>(() => DigestCredentials.ExtractParameterValueFromHeader("realm", null!));
        Assert.Throws<ArgumentException>(() =>
            DigestCredentials.ExtractParameterValueFromHeader("realm", string.Empty));
        Assert.Throws<ArgumentException>(() => DigestCredentials.ExtractParameterValueFromHeader("realm", " "));
    }

    [Fact]
    public void ExtractParameterValueFromHeader_ReturnOK()
    {
        const string wwwAuthenticateValue =
            "Digest qop=\"auth\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", stale=\"FALSE\"";

        Assert.Equal("IP Camera(K7151)",
            DigestCredentials.ExtractParameterValueFromHeader("realm", wwwAuthenticateValue));
        Assert.Equal("613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9",
            DigestCredentials.ExtractParameterValueFromHeader("nonce", wwwAuthenticateValue));
        Assert.Equal("auth",
            DigestCredentials.ExtractParameterValueFromHeader("qop", wwwAuthenticateValue));
        Assert.Null(DigestCredentials.ExtractParameterValueFromHeader("opaque", wwwAuthenticateValue));
    }

    [Fact]
    public void Create_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => DigestCredentials.Create(null!, null!, null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.Create(string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.Create(" ", null!, null!));

        Assert.Throws<ArgumentNullException>(() => DigestCredentials.Create("admin", null!, null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.Create("admin", string.Empty, null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.Create("admin", "  ", null!));

        Assert.Throws<ArgumentNullException>(() => DigestCredentials.Create("admin", "a123456789", null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.Create("admin", "a123456789", string.Empty));
        Assert.Throws<ArgumentException>(() => DigestCredentials.Create("admin", "a123456789", " "));
    }

    [Fact]
    public void Create_ReturnOK()
    {
        const string wwwAuthenticateValue =
            "Digest qop=\"auth\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", stale=\"FALSE\"";

        var credentials = DigestCredentials.Create("admin", "a123456789", wwwAuthenticateValue);
        Assert.NotNull(credentials);

        Assert.Equal("admin", credentials.Username);
        Assert.Equal("a123456789", credentials.Password);
        Assert.Equal("IP Camera(K7151)", credentials.Realm);
        Assert.Equal("613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9", credentials.Nonce);
        Assert.Equal("auth", credentials.Qop);
        Assert.Equal(1, credentials.Nc);
        Assert.NotNull(credentials.CNonce);
        Assert.True(int.Parse(credentials.CNonce) > 123399);
        Assert.Null(credentials.Opaque);
    }

    [Fact]
    public void GenerateCredentials_ReturnOK()
    {
        const string wwwAuthenticateValue =
            "Digest qop=\"auth\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", stale=\"FALSE\"";

        var credentials = DigestCredentials.Create("admin", "a123456789", wwwAuthenticateValue);
        Assert.Contains(
            "username=\"admin\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", uri=\"/test\", algorithm=MD5, qop=auth, nc=00000001",
            credentials.GenerateCredentials("/test", HttpMethod.Get));
    }

    [Fact]
    public void GetDigestCredentials_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DigestCredentials.GetDigestCredentials(null!, null!, null!, null!));
        Assert.Throws<ArgumentException>(() =>
            DigestCredentials.GetDigestCredentials(string.Empty, null!, null!, null!));
        Assert.Throws<ArgumentException>(() => DigestCredentials.GetDigestCredentials(" ", null!, null!, null!));

        Assert.Throws<ArgumentNullException>(() =>
            DigestCredentials.GetDigestCredentials("http://localhost:5000/test", null!, null!, null!));
        Assert.Throws<ArgumentException>(() =>
            DigestCredentials.GetDigestCredentials("http://localhost:5000/test", string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() =>
            DigestCredentials.GetDigestCredentials("http://localhost:5000/test", "  ", null!, null!));

        Assert.Throws<ArgumentNullException>(() =>
            DigestCredentials.GetDigestCredentials("http://localhost:5000/test", "admin", null!, null!));
        Assert.Throws<ArgumentException>(() =>
            DigestCredentials.GetDigestCredentials("http://localhost:5000/test", "admin", string.Empty, null!));
        Assert.Throws<ArgumentException>(() =>
            DigestCredentials.GetDigestCredentials("http://localhost:5000/test", "admin", "  ", null!));

        Assert.Throws<ArgumentNullException>(() =>
            DigestCredentials.GetDigestCredentials("http://localhost:5000/test", "admin", "a123456789", null!));
    }

    [Fact]
    public async Task GetDigestCredentials_InvalidOperation()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.StatusCode = 401;
            // context.Response.Headers.WWWAuthenticate =
            //     "Digest qop=\"auth\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", stale=\"FALSE\"";

            await context.Response.CompleteAsync();
        });

        await app.StartAsync();

        var ex = Assert.Throws<InvalidOperationException>(() => DigestCredentials.GetDigestCredentials(
            $"http://localhost:{port}/test",
            "admin", "a123456789", HttpMethod.Get));

        Assert.Equal("Failed to obtain digest credentials.", ex.Message);

        await app.StopAsync();
    }

    [Fact]
    public async Task GetDigestCredentials_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.WWWAuthenticate =
                "Digest qop=\"auth\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", stale=\"FALSE\"";

            await context.Response.CompleteAsync();
        });

        await app.StartAsync();

        var credentials = DigestCredentials.GetDigestCredentials($"http://localhost:{port}/test",
            "admin", "a123456789", HttpMethod.Get);

        Assert.Contains(
            "username=\"admin\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", uri=\"/test\", algorithm=MD5, qop=auth, nc=00000001",
            credentials);

        await app.StopAsync();
    }
}