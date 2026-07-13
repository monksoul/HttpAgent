// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     自动重定向管道处理器
/// </summary>
/// <param name="httpContentProcessorFactory">
///     <see cref="IHttpContentProcessorFactory" />
/// </param>
/// <param name="httpRemoteOptions">
///     <see cref="HttpRemoteOptions" />
/// </param>
internal sealed class AutoRedirectPipelineHandler(
    IHttpContentProcessorFactory httpContentProcessorFactory,
    IOptions<HttpRemoteOptions> httpRemoteOptions) : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 调用下一个处理器的委托
        var httpResponseMessage = await next();

        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 获取 HttpRemoteOptions 实例
        var remoteOptions = httpRemoteOptions.Value;

        // 初始化当前重定向次数和原始请求方法
        var redirections = 0;
        var originalHttpMethod = context.OriginalBuilder.HttpMethod!;

        // 处理请求重定向
        while (httpResponseMessage is not null &&
               Helpers.DetermineRedirectMethod(httpResponseMessage.StatusCode, originalHttpMethod,
                   out var redirectMethod) && remoteOptions.AllowAutoRedirect &&
               redirections < remoteOptions.MaximumAutomaticRedirections)
        {
            // 获取重定向地址
            var redirectUrl = httpResponseMessage.Headers.Location;

            // 空检查
            if (redirectUrl is null)
            {
                break;
            }

            // 构建新的 HttpRequestMessage 实例
            var redirectHttpRequestMessage = httpRequestBuilder
                .ConfigureForRedirect(
                    redirectUrl.IsAbsoluteUri
                        ? redirectUrl
                        : new Uri(Helpers.ParseBaseAddress(context.RequestMessage?.RequestUri), redirectUrl),
                    redirectMethod).Build(remoteOptions, httpContentProcessorFactory,
                    context.HttpClient.BaseAddress ?? remoteOptions.FallbackBaseAddress);

            // 释放前一个 HttpResponseMessage 实例
            httpResponseMessage.Dispose();

            // 重新调用发送 HTTP 请求委托
            httpResponseMessage = await context.SendAsync(context.HttpClient, redirectHttpRequestMessage,
                context.CompletionOption, context.CancellationToken);

            // 修复无效的响应内容字符编码
            httpResponseMessage.FixInvalidCharset();

            // 递增重定向次数
            redirections++;
        }

        // 更新上下文
        context.ResponseMessage = httpResponseMessage;

        return httpResponseMessage;
    }
}