// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RequestBuilderPipelineHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddOptions<HttpRemoteOptions>();
        services.TryAddSingleton<IHttpContentProcessorFactory, HttpContentProcessorFactory>();

        using var serviceProvider = services.BuildServiceProvider();

        var handler = new RequestBuilderPipelineHandler(serviceProvider,
            serviceProvider.GetRequiredService<IHttpContentProcessorFactory>(),
            serviceProvider.GetRequiredService<IOptionsMonitor<HttpRemoteOptions>>());

        Assert.NotNull(handler);
    }

    [Fact]
    public void HandlePreSendRequest_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetOnPreSendRequest(_ => throw new Exception("出错了"));

        RequestBuilderPipelineHandler.HandlePreSendRequest(httpRequestBuilder, null, new CustomRequestEventHandler(),
            new HttpRequestMessage());
    }

    [Fact]
    public async Task ExecuteAssertionsAsync_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.Asserts(u => u.RequestMethod(HttpMethod.Get));

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");
        await RequestBuilderPipelineHandler.ExecuteAssertionsAsync(httpRequestBuilder, httpRequestMessage,
            serviceProvider);

        httpRequestBuilder.UseAssertions().Asserts(u => u.RequestMethod(HttpMethod.Post));

        var exception = await Assert.ThrowsAsync<HttpAssertionException>(async () =>
            await RequestBuilderPipelineHandler.ExecuteAssertionsAsync(httpRequestBuilder, httpRequestMessage,
                serviceProvider));
        Assert.Equal("Expected request method to be POST, but found GET.", exception.Message);
    }
}