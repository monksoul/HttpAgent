// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class TokenManagementPipelineHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        services.TryAddSingleton<HttpAccessTokenManager>();
        using var serviceProvider = services.BuildServiceProvider();

        var handler = new TokenManagementPipelineHandler(serviceProvider,
            serviceProvider.GetRequiredService<HttpAccessTokenManager>());

        Assert.NotNull(handler);
    }

    [Fact]
    public void ApplyAccessToken_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        services.TryAddSingleton<HttpAccessTokenManager>();
        using var serviceProvider = services.BuildServiceProvider();

        var handler = new TokenManagementPipelineHandler(serviceProvider,
            serviceProvider.GetRequiredService<HttpAccessTokenManager>());

        Assert.Throws<ArgumentNullException>(() => handler.ApplyAccessToken(null!, null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            handler.ApplyAccessToken(HttpRequestBuilder.Get("http://localhost"), null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            handler.ApplyAccessToken(HttpRequestBuilder.Get("http://localhost"), new HttpAccessTokenProvider(), null!));
    }

    [Fact]
    public void ApplyAccessToken_ReturnOK()
    {
        var services = new ServiceCollection();
        services.TryAddSingleton<HttpAccessTokenManager>();
        using var serviceProvider = services.BuildServiceProvider();

        var handler = new TokenManagementPipelineHandler(serviceProvider,
            serviceProvider.GetRequiredService<HttpAccessTokenManager>());

        var httpRequestBuilder = HttpRequestBuilder.Get("http://localhost");
        var httpAccessTokenProvider = new HttpAccessTokenProvider();
        var httpAccessToken = new HttpAccessToken("new token", DateTimeOffset.Now.AddMinutes(10));

        handler.ApplyAccessToken(httpRequestBuilder, httpAccessTokenProvider, httpAccessToken);
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Single(httpRequestBuilder.Headers);
        Assert.Equal("new token", httpRequestBuilder.Headers["Authorization"].First());

        handler.ApplyAccessToken(httpRequestBuilder, new HttpAccessTokenProvider2(), httpAccessToken);
        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.Equal("Bearer", httpRequestBuilder.AuthenticationHeader.Scheme);
        Assert.Equal("new token", httpRequestBuilder.AuthenticationHeader.Parameter);

        var services2 = new ServiceCollection();
        services2.TryAddSingleton<HttpAccessTokenManager>();
        services2.TryAddSingleton<IHttpAccessTokenConfigurator, HttpAccessTokenConfigurator>();

        using var serviceProvider2 = services2.BuildServiceProvider();

        var handler2 = new TokenManagementPipelineHandler(serviceProvider2,
            serviceProvider2.GetRequiredService<HttpAccessTokenManager>());

        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        var httpAccessTokenProvider2 = new HttpAccessTokenProvider();
        var httpAccessToken2 = new HttpAccessToken("new token", DateTimeOffset.Now.AddMinutes(10));

        handler2.ApplyAccessToken(httpRequestBuilder2, httpAccessTokenProvider2, httpAccessToken2);
        Assert.NotNull(httpRequestBuilder2.AuthenticationHeader);
        Assert.Equal("Custom", httpRequestBuilder2.AuthenticationHeader.Scheme);
        Assert.Equal("new token", httpRequestBuilder2.AuthenticationHeader.Parameter);
    }

    private sealed class HttpAccessTokenProvider : IHttpAccessTokenProvider
    {
        /// <inheritdoc />
        public Task<HttpAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new HttpAccessToken("new token", DateTimeOffset.Now.AddMinutes(10)));
    }

    private sealed class HttpAccessTokenProvider2 : IHttpAccessTokenProvider, IHttpAccessTokenConfigurator
    {
        /// <inheritdoc />
        public void Configure(HttpRequestBuilder httpRequestBuilder, HttpAccessToken httpAccessToken) =>
            httpRequestBuilder.AddJwtBearerAuthentication(httpAccessToken.Value);

        /// <inheritdoc />
        public Task<HttpAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new HttpAccessToken("new token", DateTimeOffset.Now.AddMinutes(10)));
    }

    internal sealed class HttpAccessTokenConfigurator : IHttpAccessTokenConfigurator
    {
        /// <inheritdoc />
        public void Configure(HttpRequestBuilder httpRequestBuilder, HttpAccessToken httpAccessToken) =>
            httpRequestBuilder.AddAuthentication("Custom", httpAccessToken.Value);
    }
}