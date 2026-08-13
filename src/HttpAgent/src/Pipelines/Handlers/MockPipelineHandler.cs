// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Mock 模拟管道处理器
/// </summary>
internal sealed class MockPipelineHandler : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 检查是否存在模拟异常（优先级高于模拟响应）
        if (httpRequestBuilder.MockedException is not null)
        {
            throw httpRequestBuilder.MockedException;
        }

        // 检查是否存在模拟 HttpResponseMessage
        // ReSharper disable once InvertIf
        if (httpRequestBuilder.MockedResponse is not null)
        {
            // 更新上下文
            context.ResponseMessage = httpRequestBuilder.MockedResponse;

            return httpRequestBuilder.MockedResponse;
        }

        // 调用下一个处理器的委托
        return await next();
    }
}