// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionContextTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpAssertionContext(null!, 100, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpAssertionContext(new HttpResponseMessage(HttpStatusCode.OK), 100, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var context = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        Assert.NotNull(context);
        Assert.Null(context._cachedContent);

        Assert.NotNull(context.ResponseMessage);
        Assert.Equal(100, context.RequestDuration);
        Assert.NotNull(context.ServiceProvider);
        Assert.Equal(HttpStatusCode.OK, context.StatusCode);
        Assert.True(context.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ReadAsStringAsync_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("Hello World");

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var context = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);
        Assert.Equal("Hello World", await context.ReadAsStringAsync());
        Assert.NotNull(context._cachedContent);
        Assert.Equal("Hello World", context._cachedContent);
    }
}