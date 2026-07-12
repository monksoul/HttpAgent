// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RequestEventPipelineHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<HttpRemoteOptions>();
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, true));

        using var serviceProvider = services.BuildServiceProvider();

        var handler = new RequestEventPipelineHandler(serviceProvider,
            serviceProvider.GetRequiredService<IHttpRemoteLogger>());

        Assert.NotNull(handler);
    }

    [Fact]
    public void HandlePostReceiveResponse_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetOnPostReceiveResponse(_ => throw new Exception("出错了"));

        RequestEventPipelineHandler.HandlePostReceiveResponse(httpRequestBuilder, new CustomRequestEventHandler(),
            new HttpResponseMessage());
    }

    [Fact]
    public void HandleRequestFailed_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetOnRequestFailed((_, _) => throw new Exception("出错了"));

        RequestEventPipelineHandler.HandleRequestFailed(httpRequestBuilder, new CustomRequestEventHandler(),
            new Exception("出错了"),
            new HttpResponseMessage());
    }
}