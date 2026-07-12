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

        // 执行断言委托操作
        await ExecuteAssertionsAsync(httpRequestBuilder, httpResponseMessage, context.RequestDuration, serviceProvider);

        return httpResponseMessage;
    }

    /// <summary>
    ///     执行断言委托操作
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="requestDuration">请求耗时（毫秒）</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    internal static async Task ExecuteAssertionsAsync(HttpRequestBuilder httpRequestBuilder,
        HttpResponseMessage httpResponseMessage, long requestDuration, IServiceProvider serviceProvider)
    {
        // 检查断言是否启用且已配置委托集合
        if (httpRequestBuilder is { AssertionsEnabled: true, Assertions.Count: > 0 })
        {
            // 初始化 HttpAssertionContext 实例
            var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, requestDuration, serviceProvider);

            // 逐个调用断言委托
            foreach (var httpAssertion in httpRequestBuilder.Assertions)
            {
                await httpAssertion(httpAssertionContext);
            }
        }
    }
}