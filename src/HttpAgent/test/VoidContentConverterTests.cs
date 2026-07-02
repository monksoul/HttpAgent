// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class VoidContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new VoidContentConverter();
        Assert.NotNull(converter);
        Assert.True(
            typeof(IHttpContentConverter<VoidContent>).IsAssignableFrom(
                typeof(VoidContentConverter)));
        Assert.False(converter.KeepsResponseAlive);
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new VoidContentConverter();
        var result = converter.Read(httpResponseMessage);
        Assert.Null(result);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new VoidContentConverter();
        var result = await converter.ReadAsync(httpResponseMessage);
        Assert.Null(result);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new VoidContentConverter();
        var result = await converter.ReadAsync(httpResponseMessage, cancellationTokenSource.Token);
        Assert.Null(result);
    }

    [Fact]
    public void Read_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new VoidContentConverter();
        var result = converter.Read(typeof(string), httpResponseMessage);
        Assert.Null(result);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new VoidContentConverter();
        var result = await converter.ReadAsync(typeof(string), httpResponseMessage);
        Assert.Null(result);
    }

    [Fact]
    public async Task ReadAsync_WithType_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new VoidContentConverter();
        var result = await converter.ReadAsync(typeof(string), httpResponseMessage, cancellationTokenSource.Token);
        Assert.Null(result);
    }
}