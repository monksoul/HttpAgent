// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpResponseMessageConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new HttpResponseMessageConverter();
        Assert.NotNull(converter);
        Assert.True(
            typeof(IHttpContentConverter<HttpResponseMessage>).IsAssignableFrom(typeof(HttpResponseMessageConverter)));
        Assert.True(converter.KeepsResponseAlive);
    }

    [Fact]
    public void Read_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();

        var converter = new HttpResponseMessageConverter();
        var responseMessage = converter.Read(new HttpContentConverterContext(httpResponseMessage));
        Assert.NotNull(responseMessage);
        Assert.Equal(httpResponseMessage, responseMessage);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();

        var converter = new HttpResponseMessageConverter();
        var responseMessage = await converter.ReadAsync(new HttpContentConverterContext(httpResponseMessage));
        Assert.NotNull(responseMessage);
        Assert.Equal(httpResponseMessage, responseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new HttpResponseMessageConverter();
        var responseMessage = await converter.ReadAsync(new HttpContentConverterContext(httpResponseMessage),
            cancellationTokenSource.Token);
        Assert.NotNull(responseMessage);
        Assert.Equal(httpResponseMessage, responseMessage);
    }

    [Fact]
    public void Read_WithType_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();

        var converter = new HttpResponseMessageConverter();
        var responseMessage = converter.Read(typeof(byte[]), new HttpContentConverterContext(httpResponseMessage));
        Assert.NotNull(responseMessage);
        Assert.Equal(httpResponseMessage, responseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();

        var converter = new HttpResponseMessageConverter();
        var responseMessage =
            await converter.ReadAsync(typeof(byte[]), new HttpContentConverterContext(httpResponseMessage));
        Assert.NotNull(responseMessage);
        Assert.Equal(httpResponseMessage, responseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithType_WithCancellationToken_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new HttpResponseMessageConverter();
        var responseMessage =
            await converter.ReadAsync(typeof(byte[]), new HttpContentConverterContext(httpResponseMessage),
                cancellationTokenSource.Token);
        Assert.NotNull(responseMessage);
        Assert.Equal(httpResponseMessage, responseMessage);
    }
}