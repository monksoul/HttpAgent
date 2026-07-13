// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StatusCodePipelineHandlerTests(ITestOutputHelper output)
{
    [Fact]
    public void New_ReturnOK()
    {
        var handler = new StatusCodePipelineHandler();
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task InvokeStatusCodeHandlersAsync_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await StatusCodePipelineHandler.InvokeStatusCodeHandlersAsync(null!, null!));

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await StatusCodePipelineHandler.InvokeStatusCodeHandlersAsync(httpRequestBuilder, null!));
    }

    [Fact]
    public async Task InvokeStatusCodeHandlersAsync_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        await StatusCodePipelineHandler.InvokeStatusCodeHandlersAsync(httpRequestBuilder, httpResponseMessage);

        var i = 0;
        httpRequestBuilder.WithStatusCodeHandler(200, (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            })
            .WithStatusCodeHandler(200, (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            });

        await StatusCodePipelineHandler.InvokeStatusCodeHandlersAsync(httpRequestBuilder, httpResponseMessage);
        Assert.Equal(2, i);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var httpResponseMessage2 = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };
        var j = 0;
        httpRequestBuilder2.WithStatusCodeHandler(200, (_, _) =>
        {
            j++;
            return Task.CompletedTask;
        });
        await StatusCodePipelineHandler.InvokeStatusCodeHandlersAsync(httpRequestBuilder2, httpResponseMessage2);
        Assert.Equal(0, j);
    }

    [Theory]
    [InlineData("200", true)]
    [InlineData(200, true)]
    [InlineData(HttpStatusCode.OK, true)]
    [InlineData("*", true)]
    [InlineData('*', true)]
    [InlineData("200-300", true)]
    [InlineData("200~300", true)]
    [InlineData("100-200", true)]
    [InlineData(">=200", true)]
    [InlineData("<=200", true)]
    [InlineData("=200", true)]
    [InlineData("<201", true)]
    [InlineData(">199", true)]
    [InlineData(">200", false)]
    [InlineData(">= 200", false)]
    [InlineData("<= 200", false)]
    [InlineData("100-199", false)]
    [InlineData(">=200$", false)]
    [InlineData(HttpStatusCode.Accepted, false)]
    [InlineData(300, false)]
    [InlineData("300", false)]
    [InlineData("ok", false)]
    [InlineData("-200", false)]
    [InlineData("300-", false)]
    [InlineData(".", false)]
    [InlineData("200--300", false)]
    [InlineData("200-~300", false)]
    [InlineData("+200", false)]
    public void IsMatched200StatusCode_ReturnOK(object code, bool result) =>
        Assert.Equal(result, StatusCodePipelineHandler.IsMatchedStatusCode(code, 200));

    [Fact]
    public async Task SendCoreAsync_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await httpRemoteService.SendCoreAsync(null!, HttpCompletionOption.ResponseContentRead, null, null);
        });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await httpRemoteService.SendCoreAsync(new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/")),
                HttpCompletionOption.ResponseContentRead, null, null);
        });
        Assert.Equal("Both `sendAsyncMethod` and `sendMethod` cannot be null.", exception.Message);

        var exception2 = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _ = await httpRemoteService.SendCoreAsync(
                HttpRequestBuilder.Get("https://furion.net").SetTimeout(TimeSpan.FromSeconds(101)),
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), null);
        });
        Assert.Equal(
            "HttpTimeoutOptions's Timeout cannot be greater than HttpClient's Timeout, which defaults to 100 seconds.",
            exception2.Message);

        await serviceProvider.DisposeAsync();
    }
}