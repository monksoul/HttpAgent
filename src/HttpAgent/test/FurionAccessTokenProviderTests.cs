// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FurionAccessTokenProviderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.Equal("X-Authorization", FurionAccessTokenProvider.XAuthorizationHeaderName);
        Assert.Equal("access-token", FurionAccessTokenProvider.AccessTokenHeaderName);
        Assert.Equal("x-access-token", FurionAccessTokenProvider.XAccessTokenHeaderName);

        var services = new ServiceCollection();
        services.AddHttpRemote();
        using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<FurionAccessTokenProvider>(serviceProvider);
        Assert.NotNull(provider);
    }

    [Fact]
    public void Configure_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var services = new ServiceCollection();
        services.AddHttpRemote();
        using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<FurionAccessTokenProvider>(serviceProvider);

        provider.Configure(httpRequestBuilder,
            new HttpAccessToken("new token value", DateTimeOffset.Now.AddMinutes(20)));

        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.NotNull(httpRequestBuilder.OnPostReceiveResponse);
    }

    [Fact]
    public async Task GetAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        await using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<FurionAccessTokenProvider>(serviceProvider);

        var accessToken =
            await provider.GetAsync(new HttpAccessTokenContext(null, provider), CancellationToken.None);
        Assert.Null(accessToken);
    }

    [Fact]
    public async Task RefreshAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        await using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<FurionAccessTokenProvider>(serviceProvider);

        var accessToken =
            await provider.RefreshAsync(new HttpAccessTokenContext(null, provider), null, CancellationToken.None);
        Assert.Null(accessToken);

        var originalToken = new HttpAccessToken("new token value", DateTimeOffset.Now.AddMinutes(20))
        {
            RefreshToken = "my_refresh_token", Scheme = "Bearer", Items = { ["custom_key"] = "custom_value" }
        };

        var refreshedToken = await provider.RefreshAsync(new HttpAccessTokenContext(null, provider),
            originalToken, CancellationToken.None);

        Assert.NotNull(refreshedToken);
        Assert.Equal("new token value", refreshedToken.Value);
        Assert.Equal(DateTimeOffset.MinValue, refreshedToken.ExpiresAt);
        Assert.Equal("my_refresh_token", refreshedToken.RefreshToken);
        Assert.Equal("Bearer", refreshedToken.Scheme);
        Assert.Equal("custom_value", refreshedToken.Items["custom_key"]);
    }

    [Fact]
    public async Task ShouldRefreshAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        await using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<FurionAccessTokenProvider>(serviceProvider);

        Assert.False(await provider.ShouldRefreshAsync(new HttpAccessTokenContext(null, provider),
            new HttpResponseMessage(HttpStatusCode.OK), TestContext.Current.CancellationToken));

        Assert.True(await provider.ShouldRefreshAsync(new HttpAccessTokenContext(null, provider),
            new HttpResponseMessage(HttpStatusCode.Unauthorized), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Configure_OnPostReceiveResponse_InvalidToken_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        await using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<FurionAccessTokenProvider>(serviceProvider);

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetHttpClientName("test_client");

        var accessTokenManager = serviceProvider.GetRequiredService<IHttpAccessTokenManager>();

        var initialToken =
            new HttpAccessToken("old_token", DateTimeOffset.UtcNow.AddMinutes(10)) { RefreshToken = "old_refresh" };
        await accessTokenManager.SetAsync(httpRequestBuilder.HttpClientName, initialToken,
            TestContext.Current.CancellationToken);

        provider.Configure(httpRequestBuilder, initialToken);

        var invalidResponse = new HttpResponseMessage(HttpStatusCode.OK);
        invalidResponse.Headers.Add("access-token", "invalid_token");
        invalidResponse.Headers.Add("x-access-token", "new_refresh");

        await httpRequestBuilder.OnPostReceiveResponse!.TryInvokeAsync(invalidResponse, CancellationToken.None);

        var cachedToken =
            await accessTokenManager.GetAsync(httpRequestBuilder.HttpClientName, TestContext.Current.CancellationToken);
        Assert.NotNull(cachedToken);
        Assert.Equal("old_token", cachedToken.Value);
    }
}