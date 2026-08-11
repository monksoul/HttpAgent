// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionBuilderRequestAssertionsTests
{
    [Fact]
    public void RequestUri_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.RequestUri(null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestUri(string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestUri(" "));
    }

    [Fact]
    public async Task RequestUri_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.RequestUri("https://furion.net/");
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://furion.net/");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://furion.net/api");
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request URI to be 'https://furion.net/', but found 'https://furion.net/api'.",
            exception.Message);
    }

    [Fact]
    public void RequestMethod_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.RequestMethod(null!));
    }

    [Fact]
    public async Task RequestMethod_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.RequestMethod(HttpMethod.Post);
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://furion.net/");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://furion.net/");
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request method to be POST, but found GET.",
            exception.Message);
    }

    [Fact]
    public void RequestHeaderExists_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.RequestHeaderExists(null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestHeaderExists(string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestHeaderExists(" "));
    }

    [Fact]
    public async Task RequestHeaderExists_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.RequestHeaderExists("Authorization");
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer token");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpRequestMessage = new HttpRequestMessage();
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request header 'Authorization' to exist, but it was not found.",
            exception.Message);
    }

    [Fact]
    public void RequestHeaderEquals_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.RequestHeaderEquals(null!, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestHeaderEquals(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestHeaderEquals(" ", null!));

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.RequestHeaderEquals("Authorization", null!));
    }

    [Fact]
    public async Task RequestHeaderEquals_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.RequestHeaderEquals("Authorization", "Bearer token");
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer token");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpRequestMessage.Headers.Remove("Authorization");
        httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer another");
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request header 'Authorization' to be 'Bearer token', but found 'Bearer another'.",
            exception.Message);
    }

    [Fact]
    public void RequestHeaderContains_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.RequestHeaderContains(null!, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestHeaderContains(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestHeaderContains(" ", null!));

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.RequestHeaderContains("Authorization", null!));
        Assert.Throws<ArgumentException>(() =>
            httpAssertionBuilder.RequestHeaderContains("Authorization", string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.RequestHeaderContains("Authorization", " "));
    }

    [Fact]
    public async Task RequestHeaderContains_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.RequestHeaderContains("Authorization", "Bearer");
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Bearer token");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpRequestMessage.Headers.Remove("Authorization");
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request header 'Authorization' to contain 'Bearer', but the header was not found.",
            exception.Message);

        httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", "Basic abc");
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var exception2 =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal(
            "Expected request header 'Authorization' to contain 'Bearer', but actual values were: [Basic abc].",
            exception2.Message);
    }

    [Fact]
    public void RequestContentContains_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.RequestContentContains(null!, TestContext.Current.CancellationToken));
        Assert.Throws<ArgumentException>(() =>
            httpAssertionBuilder.RequestContentContains(string.Empty, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RequestContentContains_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.RequestContentContains("Hello", TestContext.Current.CancellationToken);
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StringContent("Hello World!");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpRequestMessage.Content = new StringContent("Furion YYDS!");
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request content to contain 'Hello', but it was not found.",
            exception.Message);
    }

    [Fact]
    public void RequestContentEquals_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.RequestContentEquals(null!, TestContext.Current.CancellationToken));
        Assert.Throws<ArgumentException>(() =>
            httpAssertionBuilder.RequestContentEquals(string.Empty, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RequestContentEquals_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.RequestContentEquals("Hello World!", TestContext.Current.CancellationToken);
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StringContent("Hello World!");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpRequestMessage.Content = new StringContent("Furion YYDS!");
        httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request content to be 'Hello World!', but found 'Furion YYDS!'.",
            exception.Message);
    }

    [Fact]
    public void RequestSatisfies_Sync_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.RequestSatisfies((Action<HttpRequestMessage>)null!));
    }

    [Fact]
    public async Task RequestSatisfies_Sync_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://furion.net/");
        httpAssertionBuilder.RequestSatisfies(r => Assert.Equal(HttpMethod.Get, r.Method));
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpAssertionBuilder.RequestSatisfies(r => throw new InvalidOperationException("test error"));
        assertion = httpAssertionBuilder._requestAssertions[1];

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("test error", exception.Message);
    }

    [Fact]
    public void RequestSatisfies_Async_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.RequestSatisfies(null!));
    }

    [Fact]
    public async Task RequestSatisfies_Async_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://furion.net/");
        httpAssertionBuilder.RequestSatisfies(async r =>
        {
            await Task.Yield();
            Assert.Equal(HttpMethod.Post, r.Method);
        });
        Assert.Single(httpAssertionBuilder._requestAssertions);

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        var assertion = httpAssertionBuilder._requestAssertions[0];
        await assertion(httpAssertionContext);

        httpAssertionBuilder.RequestSatisfies(r => throw new InvalidOperationException("async error"));
        assertion = httpAssertionBuilder._requestAssertions[1];

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("async error", exception.Message);
    }
}