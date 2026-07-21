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
            serviceProvider.GetRequiredService<IOptions<HttpRemoteOptions>>());

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
}