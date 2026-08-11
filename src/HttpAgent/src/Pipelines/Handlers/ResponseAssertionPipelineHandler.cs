// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     响应断言管道处理器
/// </summary>
/// <param name="serviceProvider">
///     <see cref="IServiceProvider" />
/// </param>
internal sealed class ResponseAssertionPipelineHandler(IServiceProvider serviceProvider) : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 调用下一个处理器的委托
        var httpResponseMessage = await next();

        // 空检查
        if (httpResponseMessage is null)
        {
            return null;
        }

        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 执行响应断言委托操作
        await ExecuteAssertionsAsync(httpRequestBuilder, httpResponseMessage, context.RequestMessage,
            context.RequestDuration, serviceProvider);

        return httpResponseMessage;
    }

    /// <summary>
    ///     执行响应断言委托操作
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="requestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="requestDuration">请求耗时（毫秒）</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    internal static async Task ExecuteAssertionsAsync(HttpRequestBuilder httpRequestBuilder,
        HttpResponseMessage httpResponseMessage, HttpRequestMessage? requestMessage, long requestDuration,
        IServiceProvider serviceProvider)
    {
        // 检查是否配置了响应断言委托集合
        if (httpRequestBuilder.ResponseAssertions is { Count: > 0 })
        {
            // 初始化 HttpAssertionContext 实例
            var httpAssertionContext =
                new HttpAssertionContext(httpResponseMessage, requestMessage, requestDuration, serviceProvider);

            // 逐个调用响应断言委托
            foreach (var httpAssertion in httpRequestBuilder.ResponseAssertions)
            {
                await httpAssertion(httpAssertionContext);
            }
        }
    }
}