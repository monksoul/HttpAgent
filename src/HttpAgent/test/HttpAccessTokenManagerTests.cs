// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAccessTokenManagerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var tokenManager = new HttpAccessTokenManager();
        Assert.NotNull(tokenManager._httpClientNameCaches);
        Assert.Empty(tokenManager._httpClientNameCaches);
    }

    [Fact]
    public async Task GetOrRefreshAsync_Invalid_Parameters()
    {
        var tokenManager = new HttpAccessTokenManager();
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            tokenManager.GetOrRefreshAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetOrRefreshAsync_ReturnOK()
    {
        var tokenManager = new HttpAccessTokenManager();
        Assert.Empty(tokenManager._httpClientNameCaches);

        var tokenProvider = new HttpAccessTokenProvider();
        var accessToken =
            await tokenManager.GetOrRefreshAsync(new HttpAccessTokenContext(null, tokenProvider),
                CancellationToken.None);
        Assert.Single(tokenManager._httpClientNameCaches);
        Assert.NotNull(accessToken);
        Assert.Equal("new token", accessToken.Value);
        Assert.False(accessToken.IsExpired());

        var accessToken2 =
            await tokenManager.GetOrRefreshAsync(new HttpAccessTokenContext(null, tokenProvider),
                CancellationToken.None);
        Assert.Single(tokenManager._httpClientNameCaches);
        Assert.Same(accessToken, accessToken2);
    }

    [Fact]
    public async Task ForceRefreshAsync_Invalid_Parameters()
    {
        var tokenManager = new HttpAccessTokenManager();
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            tokenManager.ForceRefreshAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ForceRefreshAsync_ReturnOK()
    {
        var tokenManager = new HttpAccessTokenManager();
        Assert.Empty(tokenManager._httpClientNameCaches);

        var tokenProvider = new HttpAccessTokenProvider();
        var accessToken =
            await tokenManager.ForceRefreshAsync(new HttpAccessTokenContext(null, tokenProvider),
                CancellationToken.None);
        Assert.Single(tokenManager._httpClientNameCaches);
        Assert.NotNull(accessToken);
        Assert.Equal("new token", accessToken.Value);
        Assert.False(accessToken.IsExpired());

        var accessToken2 =
            await tokenManager.ForceRefreshAsync(new HttpAccessTokenContext(null, tokenProvider),
                CancellationToken.None);
        Assert.Single(tokenManager._httpClientNameCaches);
        Assert.NotSame(accessToken, accessToken2);
    }

    [Fact]
    public void AccessTokenCache_New_ReturnOK()
    {
        var accessTokenCache = new HttpAccessTokenManager.AccessTokenCache();
        Assert.NotNull(accessTokenCache._refreshLock);
        Assert.Equal(1, accessTokenCache._refreshLock.CurrentCount);
        Assert.Null(accessTokenCache._current);
        Assert.Null(accessTokenCache.Current);
    }

    private sealed class HttpAccessTokenProvider : IHttpAccessTokenProvider
    {
        /// <inheritdoc />
        public Task<HttpAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new HttpAccessToken("new token", DateTimeOffset.Now.AddMinutes(10)));
    }
}