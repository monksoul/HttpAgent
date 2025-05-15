// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteUtilityTests
{
    [Fact]
    public void AllSslProtocols_ReturnOK()
    {
#pragma warning disable SYSLIB0039
#pragma warning disable CS0618 // 类型或成员已过时
        Assert.Equal(
            SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls12 |
            SslProtocols.Tls13 | SslProtocols.None,
#pragma warning restore CS0618 // 类型或成员已过时
#pragma warning restore SYSLIB0039
            HttpRemoteUtility.AllSslProtocols);
    }

    [Fact]
    public void IgnoreSslErrors_ReturnOK()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpRemoteUtility.IgnoreSslErrors
        };

        Assert.NotNull(handler.ServerCertificateCustomValidationCallback);
    }

    [Fact]
    public void IgnoreSocketSslErrors_ReturnOK()
    {
        var handler = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = HttpRemoteUtility.IgnoreSocketSslErrors
            }
        };

        Assert.NotNull(handler.SslOptions.RemoteCertificateValidationCallback);
    }

    [Fact]
    public async Task IPAddressConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = (context, token) =>
                HttpRemoteUtility.IPAddressConnectCallback(AddressFamily.Unspecified, context, token)
        });

        var response = await httpClient.GetAsync("https://www.baidu.com");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task IPv4ConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = HttpRemoteUtility.IPv4ConnectCallback
        });

        var response = await httpClient.GetAsync("https://www.baidu.com");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task IPv6ConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = HttpRemoteUtility.IPv6ConnectCallback
        });

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var response = await httpClient.GetAsync("https://www.baidu.com");
            response.EnsureSuccessStatusCode();
        });
    }

    [Fact]
    public async Task UnspecifiedConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = HttpRemoteUtility.UnspecifiedConnectCallback
        });

        var response = await httpClient.GetAsync("https://www.baidu.com");
        response.EnsureSuccessStatusCode();
    }
}