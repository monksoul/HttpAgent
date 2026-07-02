// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StreamContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new StreamContentConverter();
        Assert.NotNull(converter);
        Assert.True(typeof(IHttpContentConverter<Stream>).IsAssignableFrom(typeof(StreamContentConverter)));
        Assert.True(converter.KeepsResponseAlive);
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var memoryStream = new MemoryStream("furion"u8.ToArray());
        using var streamContent = new StreamContent(memoryStream);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = streamContent;

        var converter = new StreamContentConverter();
        var stream = converter.Read(httpResponseMessage);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var result = reader.ReadToEnd();
        Assert.Equal("furion", result);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var memoryStream = new MemoryStream("furion"u8.ToArray());
        using var streamContent = new StreamContent(memoryStream);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = streamContent;

        var converter = new StreamContentConverter();
        var stream = await converter.ReadAsync(httpResponseMessage);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var result = await reader.ReadToEndAsync();
        Assert.Equal("furion", result);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var memoryStream = new MemoryStream("furion"u8.ToArray());
        using var streamContent = new StreamContent(memoryStream);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = streamContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new StreamContentConverter();
        var stream = await converter.ReadAsync(httpResponseMessage, cancellationTokenSource.Token);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var result = await reader.ReadToEndAsync(cancellationTokenSource.Token);
        Assert.Equal("furion", result);
    }

    [Fact]
    public void Read_WithType_ReturnOK()
    {
        using var memoryStream = new MemoryStream("furion"u8.ToArray());
        using var streamContent = new StreamContent(memoryStream);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = streamContent;

        var converter = new StreamContentConverter();
        var stream = converter.Read(typeof(Stream), httpResponseMessage);
        Assert.NotNull(stream);

        using var reader = new StreamReader((Stream)stream, Encoding.UTF8);
        var result = reader.ReadToEnd();
        Assert.Equal("furion", result);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        using var memoryStream = new MemoryStream("furion"u8.ToArray());
        using var streamContent = new StreamContent(memoryStream);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = streamContent;

        var converter = new StreamContentConverter();
        var stream = await converter.ReadAsync(typeof(Stream), httpResponseMessage);
        Assert.NotNull(stream);

        using var reader = new StreamReader((Stream)stream, Encoding.UTF8);
        var result = await reader.ReadToEndAsync();
        Assert.Equal("furion", result);
    }

    [Fact]
    public async Task ReadAsync_WithType_WithCancellationToken_ReturnOK()
    {
        using var memoryStream = new MemoryStream("furion"u8.ToArray());
        using var streamContent = new StreamContent(memoryStream);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = streamContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new StreamContentConverter();
        var stream = await converter.ReadAsync(typeof(Stream), httpResponseMessage, cancellationTokenSource.Token);
        Assert.NotNull(stream);

        using var reader = new StreamReader((Stream)stream, Encoding.UTF8);
        var result = await reader.ReadToEndAsync(cancellationTokenSource.Token);
        Assert.Equal("furion", result);
    }
}