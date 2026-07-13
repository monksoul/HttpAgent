// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     超时控制管道处理器
/// </summary>
internal sealed class TimeoutPipelineHandler : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 获取 HttpTimeoutOptions 实例（空检查）
        if (httpRequestBuilder.TimeoutOptions is not { Timeout: not null } httpTimeoutOptions)
        {
            // 调用下一个处理器的委托
            return await next();
        }

        // 如果超时设置为无限，跳过超时处理
        if (httpTimeoutOptions.Timeout.Value == Timeout.InfiniteTimeSpan)
        {
            // 调用下一个处理器的委托
            return await next();
        }

        // 确保 HttpTimeoutOptions 的 Timeout 属性值小于 HttpClient 的 Timeout 属性值（默认 100秒）
        if (httpTimeoutOptions.Timeout.Value > context.HttpClient.Timeout)
        {
            throw new InvalidOperationException(
                "HttpTimeoutOptions's Timeout cannot be greater than HttpClient's Timeout, which defaults to 100 seconds.");
        }

        // 创建关联的超时 Token 标识
        using var timeoutCancellationTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        var timeoutCancellationToken = timeoutCancellationTokenSource.Token;

        // 定义标志位，用于判断是否引发了超时操作
        var isTimeoutTriggered = false;

        // 调用超时发生时要执行的操作
        if (httpTimeoutOptions.OnTimeout is not null)
        {
            timeoutCancellationToken.Register(httpTimeoutOptions.OnTimeout.TryInvoke);
        }

        // 注册回调，用于标记是否是超时触发的取消
        timeoutCancellationToken.Register(() => isTimeoutTriggered = true);

        // 延迟指定时间后取消任务
        timeoutCancellationTokenSource.CancelAfter(httpTimeoutOptions.Timeout.Value);

        // 获取原始取消令牌
        var originalToken = context.CancellationToken;

        // 更新上下文（替换）
        context.CancellationToken = timeoutCancellationToken;

        try
        {
            // 调用下一个处理器的委托
            return await next();
        }
        // 检查是否是超时导致的取消，如果是则抛出 TaskCanceledException(TimeoutException) 超时异常
        catch (OperationCanceledException ex) when (isTimeoutTriggered && !originalToken.IsCancellationRequested)
        {
            throw new TaskCanceledException(
                $"The request was canceled due to the configured HttpRequestBuilder.Timeout of {httpTimeoutOptions.Timeout.Value.TotalSeconds:0.###} seconds elapsing.",
                new TimeoutException("The operation was canceled.", ex));
        }
        finally
        {
            // 同步上下文
            context.CancellationToken = originalToken;
        }
    }
}