// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAccessTokenContextTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpAccessTokenContext(null, null!));
        Assert.Throws<ArgumentNullException>(() => new HttpAccessTokenContext("github", null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var tokenContext = new HttpAccessTokenContext(null, new HttpAccessTokenProvider());
        Assert.NotNull(tokenContext.HttpClientName);
        Assert.Equal(string.Empty, tokenContext.HttpClientName);
        Assert.NotNull(tokenContext.HttpAccessTokenProvider);
        Assert.NotNull(tokenContext.Items);
        Assert.Empty(tokenContext.Items);

        var tokenContext2 = new HttpAccessTokenContext("github", new HttpAccessTokenProvider());
        Assert.NotNull(tokenContext2.HttpClientName);
        Assert.Equal("github", tokenContext2.HttpClientName);
        Assert.NotNull(tokenContext2.HttpAccessTokenProvider);
    }

    private sealed class HttpAccessTokenProvider : IHttpAccessTokenProvider
    {
        /// <inheritdoc />
        public Task<HttpAccessToken?>
            GetTokenAsync(HttpAccessTokenContext context, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}