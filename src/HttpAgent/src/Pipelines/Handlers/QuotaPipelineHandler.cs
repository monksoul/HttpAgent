// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     配额管道处理器
/// </summary>
/// <param name="serviceProvider">
///     <see cref="IServiceProvider" />
/// </param>
/// <param name="quotaManager">
///     <see cref="IHttpQuotaManager" />
/// </param>
internal sealed class QuotaPipelineHandler(IServiceProvider serviceProvider, IHttpQuotaManager quotaManager)
    : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 获取配额键
        var quotaKey = httpRequestBuilder.QuotaKey;

        // 检查是否指定配额键
        if (string.IsNullOrWhiteSpace(quotaKey))
        {
            // 调用下一个处理器的委托
            return await next();
        }

        // 获取当前 HttpClient 实例的配置名称
        var httpClientName = httpRequestBuilder.HttpClientName;

        // 获取当前 HttpClient 实例的配置名称的配置选项
        var httpClientOptions = HttpRemoteUtility.ResolveHttpClientOptions(serviceProvider, httpClientName);

        // 获取当前 HttpClient 实例的配置名称的接口调用配额限制配置
        var quotaLimits = httpClientOptions?.QuotaLimits;

        // 根据配额键查找是否包含接口调用配额限制配置
        if (quotaLimits is null || !quotaLimits.TryGetValue(quotaKey, out var quotaLimit))
        {
            // 调用下一个处理器的委托
            return await next();
        }

        // 尝试递增调用计数，并检查是否超过配额
        if (!quotaManager.TryIncrement(httpClientName, quotaKey, quotaLimit, out var current))
        {
            throw new InvalidOperationException(
                $"Request aborted due to quota limit. Key: '{quotaKey}', Strategy: '{quotaLimit.Strategy}', Max: {quotaLimit.MaxCount}, Attempted: {current}.");
        }

        // 调用下一个处理器的委托
        return await next();
    }
}