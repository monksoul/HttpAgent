// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     核心 HTTP 发送管道处理器
/// </summary>
internal sealed class SendCorePipelineHandler : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 调用发送 HTTP 请求委托
        var httpResponseMessage = await context.SendAsync(context.HttpClient, context.RequestMessage!,
            context.CompletionOption, context.CancellationToken);

        // 修复无效的响应内容字符编码
        httpResponseMessage.FixInvalidCharset();

        // 更新上下文
        context.ResponseMessage = httpResponseMessage;

        return httpResponseMessage;
    }
}