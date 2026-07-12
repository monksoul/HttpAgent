// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     请求分析日志管道处理器
/// </summary>
/// <param name="logger">
///     <see cref="IHttpRemoteLogger" />
/// </param>
/// <param name="httpRemoteOptions">
///     <see cref="HttpRemoteOptions" />
/// </param>
internal sealed class RequestProfilerPipelineHandler(
    IHttpRemoteLogger logger,
    IOptions<HttpRemoteOptions> httpRemoteOptions) : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 检查是否启用请求分析工具
        if (!httpRequestBuilder.ProfilerEnabled)
        {
            // 调用下一个处理器的委托
            return await next();
        }

        // 标记已打印，解决重复打印问题
        context.RequestMessage!.Options.TryAdd(Constants.PROFILER_PRINTED_KEY, "TRUE");

        // 初始化 HttpRemoteAnalyzer 实例
        var httpRemoteAnalyzer = httpRequestBuilder.ProfilerPredicate is not null ? new HttpRemoteAnalyzer() : null;

        // 存入上下文
        context.Items["ProfilerAnalyzer"] = httpRemoteAnalyzer;

        // 记录请求信息
        await ProfilerDelegatingHandler.LogRequestAsync(logger, httpRemoteOptions.Value, context.RequestMessage,
            httpRemoteAnalyzer, context.HttpClient, context.CancellationToken);

        // 调用下一个处理器的委托
        return await next();
    }
}