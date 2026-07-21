// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FurionAccessTokenProviderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var provider = new FurionAccessTokenProvider();
        Assert.NotNull(provider);
        Assert.True(typeof(IHttpAccessTokenProvider).IsAssignableFrom(typeof(FurionAccessTokenProvider)));
        Assert.True(typeof(IHttpAccessTokenConfigurator).IsAssignableFrom(typeof(FurionAccessTokenProvider)));
    }

    [Fact]
    public void Configure_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var provider = new FurionAccessTokenProvider();
        provider.Configure(httpRequestBuilder,
            new HttpAccessToken("new token value", DateTimeOffset.Now.AddMinutes(20)));

        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.NotNull(httpRequestBuilder._onPostReceiveResponse);
        Assert.NotNull(httpRequestBuilder.OnPostReceiveResponse);
    }

    [Fact]
    public async Task GetTokenAsync_ReturnOK()
    {
        var provider = new FurionAccessTokenProvider();
        var accessToken =
            await provider.GetTokenAsync(new HttpAccessTokenContext(null, provider), CancellationToken.None);
        Assert.Null(accessToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_ReturnOK()
    {
        var provider = new FurionAccessTokenProvider();
        var accessToken =
            await provider.RefreshTokenAsync(new HttpAccessTokenContext(null, provider), null, CancellationToken.None);
        Assert.Null(accessToken);

        var accessToken2 =
            await provider.RefreshTokenAsync(new HttpAccessTokenContext(null, provider),
                new HttpAccessToken("new token value", DateTimeOffset.Now.AddMinutes(20)), CancellationToken.None);
        Assert.NotNull(accessToken2);
        Assert.Equal("new token value", accessToken2.Value);
    }
}