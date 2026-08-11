// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionContextTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpAssertionContext(null, null, 100, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpAssertionContext(new HttpResponseMessage(HttpStatusCode.OK), null, 100, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://furion.net/");
        var context = new HttpAssertionContext(httpResponseMessage, httpRequestMessage, 100, serviceProvider);

        Assert.NotNull(context);
        Assert.Null(context._cachedResponseContent);
        Assert.Null(context._cachedRequestContent);

        Assert.NotNull(context.ResponseMessage);
        Assert.Same(httpResponseMessage, context.ResponseMessage);
        Assert.Same(httpRequestMessage, context.RequestMessage);
        Assert.Equal(100, context.RequestDuration);
        Assert.NotNull(context.ServiceProvider);
        Assert.Equal(HttpStatusCode.OK, context.StatusCode);
        Assert.True(context.IsSuccessStatusCode);
    }

    [Fact]
    public void New_WithNullResponseMessage_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://furion.net/");
        var context = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

        Assert.Null(context.ResponseMessage);
        Assert.Same(httpRequestMessage, context.RequestMessage);
        Assert.Equal(0, context.RequestDuration);
        Assert.Equal(default, context.StatusCode);
        Assert.False(context.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ReadResponseAsStringAsync_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("Hello World");

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);
        Assert.Equal("Hello World", await context.ReadResponseAsStringAsync(TestContext.Current.CancellationToken));
        Assert.NotNull(context._cachedResponseContent);
        Assert.Equal("Hello World", context._cachedResponseContent);
    }

    [Fact]
    public async Task ReadResponseAsStringAsync_CachesAndReturnsSameResult_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("data");

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(httpResponseMessage, null, 0, serviceProvider);
        var first = await context.ReadResponseAsStringAsync(TestContext.Current.CancellationToken);
        var second = await context.ReadResponseAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal("data", first);
        Assert.Same(first, second);
    }

    [Fact]
    public async Task ReadResponseAsStringAsync_WithNullResponseMessage_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(null, new HttpRequestMessage(), 0, serviceProvider);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            context.ReadResponseAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ReadRequestAsStringAsync_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StringContent("request body");

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);
        var result = await context.ReadRequestAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal("request body", result);
        Assert.NotNull(context._cachedRequestContent);
        Assert.Equal("request body", context._cachedRequestContent);
    }

    [Fact]
    public async Task ReadRequestAsStringAsync_CachesAndReturnsSameResult_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StringContent("data");

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);
        var first = await context.ReadRequestAsStringAsync(TestContext.Current.CancellationToken);
        var second = await context.ReadRequestAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal("data", first);
        Assert.Same(first, second);
    }

    [Fact]
    public async Task ReadRequestAsStringAsync_WithNullContent_ReturnOK()
    {
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(null, new HttpRequestMessage(), 0, serviceProvider);
        var result = await context.ReadRequestAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task ReadRequestAsStringAsync_WithNonSeekableStream_StillReadsContent_ReturnOK()
    {
        var rawBytes = "non-seekable stream"u8.ToArray();
        var stream = new NonSeekableMemoryStream(rawBytes);
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StreamContent(stream);

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);
        var result = await context.ReadRequestAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal("non-seekable stream", result);
    }

    private sealed class NonSeekableMemoryStream(byte[] data) : MemoryStream(data)
    {
        public override bool CanSeek => false;
    }
}