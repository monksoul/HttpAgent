// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ResponseAssertionPipelineHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var handler = new ResponseAssertionPipelineHandler(serviceProvider);
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task ExecuteAssertionsAsync_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.Asserts(u => u.StatusCode(200));

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
        await ResponseAssertionPipelineHandler.ExecuteAssertionsAsync(httpRequestBuilder, httpResponseMessage, 100,
            serviceProvider);

        httpRequestBuilder.UseAssertions().Asserts(u => u.StatusCode(200));

        var exception = await Assert.ThrowsAsync<HttpAssertionException>(async () =>
            await ResponseAssertionPipelineHandler.ExecuteAssertionsAsync(httpRequestBuilder, httpResponseMessage, 100,
                serviceProvider));
        Assert.Equal("Expected status code to be 200, but found 204.", exception.Message);
    }
}