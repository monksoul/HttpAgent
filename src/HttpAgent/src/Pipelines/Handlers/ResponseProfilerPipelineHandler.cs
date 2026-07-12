// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     响应分析日志管道处理器
/// </summary>
/// <param name="logger">
///     <see cref="IHttpRemoteLogger" />
/// </param>
/// <param name="httpRemoteOptions">
///     <see cref="HttpRemoteOptions" />
/// </param>
internal sealed class ResponseProfilerPipelineHandler(
    IHttpRemoteLogger logger,
    IOptions<HttpRemoteOptions> httpRemoteOptions) : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 初始化 Stopwatch 实例并开启计时操作
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 调用下一个处理器的委托
            return await next();
        }
        finally
        {
            // 停止计时并记录耗时
            stopwatch.Stop();

            // 获取请求耗时（更新上下文）
            context.RequestDuration = stopwatch.ElapsedMilliseconds;

            // 获取当前 HttpRequestBuilder 实例
            var httpRequestBuilder = context.Builder;

            // 检查是否启用请求分析工具
            if (context.ResponseMessage is not null && httpRequestBuilder.ProfilerEnabled)
            {
                // 解析 HttpRemoteAnalyzer 实例
                var httpRemoteAnalyzer = context.Items.TryGetValue("ProfilerAnalyzer", out var analyzer)
                    ? analyzer as HttpRemoteAnalyzer
                    : null;

                // 记录响应信息
                await ProfilerDelegatingHandler.LogResponseAsync(logger, httpRemoteOptions.Value,
                    context.ResponseMessage, context.RequestDuration, httpRemoteAnalyzer, context.CancellationToken);

                // 调用请求分析工具委托
                httpRequestBuilder.ProfilerPredicate?.TryInvoke(httpRemoteAnalyzer!);
            }
        }
    }
}