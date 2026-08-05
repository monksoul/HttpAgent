// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WeChatAccessTokenProviderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.Equal("access_token", WeChatAccessTokenProvider.AccessTokenKey);
        Assert.Equal("expires_in", WeChatAccessTokenProvider.ExpiresInKey);

        var services = new ServiceCollection();
        services.AddHttpRemote();
        using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<WeChatAccessTokenProvider>(serviceProvider, "AppID", "AppSecret");
        Assert.NotNull(provider);
    }

    [Fact]
    public void Configure_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<WeChatAccessTokenProvider>(serviceProvider, "AppID", "AppSecret");

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        provider.Configure(httpRequestBuilder, new HttpAccessToken("token", DateTimeOffset.Now.AddSeconds(20)));
    }

    [Fact]
    public async Task GetAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        await using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<WeChatAccessTokenProvider>(serviceProvider, "AppID", "AppSecret");

        var accessToken = await provider.GetAsync(new HttpAccessTokenContext(null, provider),
            TestContext.Current.CancellationToken);
        Assert.Null(accessToken);
    }

    [Fact]
    public async Task ShouldRefreshAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        await using var serviceProvider = services.BuildServiceProvider();

        var provider =
            ActivatorUtilities.CreateInstance<WeChatAccessTokenProvider>(serviceProvider, "AppID", "AppSecret");

        var context = new HttpAccessTokenContext(null, provider);
        Assert.True(await provider.ShouldRefreshAsync(context, new HttpResponseMessage(HttpStatusCode.Unauthorized),
            TestContext.Current.CancellationToken));
        Assert.True(await provider.ShouldRefreshAsync(context, new HttpResponseMessage(HttpStatusCode.Forbidden),
            TestContext.Current.CancellationToken));

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        Assert.False(await provider.ShouldRefreshAsync(context, httpResponseMessage,
            TestContext.Current.CancellationToken));

        httpResponseMessage.Content =
            new StringContent("{\"errcode\":40001,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
                new MediaTypeHeaderValue("application/json"));
        Assert.True(await provider.ShouldRefreshAsync(context, httpResponseMessage,
            TestContext.Current.CancellationToken));
        Assert.Equal("{\"errcode\":40001,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
            await httpResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        httpResponseMessage.Content =
            new StringContent("{\"errcode\":40014,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
                new MediaTypeHeaderValue("application/json"));
        Assert.True(await provider.ShouldRefreshAsync(context, httpResponseMessage,
            TestContext.Current.CancellationToken));
        Assert.Equal("{\"errcode\":40014,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
            await httpResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        httpResponseMessage.Content =
            new StringContent("{\"errcode\":42001,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
                new MediaTypeHeaderValue("application/json"));
        Assert.True(await provider.ShouldRefreshAsync(context, httpResponseMessage,
            TestContext.Current.CancellationToken));
        Assert.Equal("{\"errcode\":42001,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
            await httpResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        httpResponseMessage.Content =
            new StringContent("{\"errcode\":40013,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
                new MediaTypeHeaderValue("application/json"));
        Assert.False(await provider.ShouldRefreshAsync(context, httpResponseMessage,
            TestContext.Current.CancellationToken));
        Assert.Equal("{\"errcode\":40013,\"errmsg\":\"invalid appid rid: 6a72d87d-02a061cf-740bf233\"}",
            await httpResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}