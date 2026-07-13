// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="RetryAttribute" /> 特性提取器
/// </summary>
internal sealed class RetryDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否贴有 [Retry] 特性
        if (!context.IsMethodDefined<RetryAttribute>(out var retryAttribute, true))
        {
            return;
        }

        // 初始化 HttpRetryOptions 实例
        var httpRetryOptions = new HttpRetryOptions().SetMaxRetries(retryAttribute.MaxRetries)
            .SetUseExponentialBackoff(retryAttribute.UseExponentialBackoff)
            .SetRetryIndefinitely(retryAttribute.RetryIndefinitely);

        // 检查是否配置了自定义重试间隔数组
        if (retryAttribute.RetryIntervals is { Length: > 0 })
        {
            httpRetryOptions.SetRetryIntervals(retryAttribute.RetryIntervals);
        }
        else
        {
            httpRetryOptions.SetRetryInterval(retryAttribute.RetryInterval);
        }

        // 检查是否配置了需要重试的 HTTP 状态码集合
        if (retryAttribute.RetryStatusCodes is { Length: > 0 })
        {
            httpRetryOptions.AddRetryStatusCodes(retryAttribute.RetryStatusCodes);
        }

        // 检查是否配置了需要重试的异常类型集合
        if (retryAttribute.RetryExceptionTypes is { Length: > 0 })
        {
            httpRetryOptions.AddRetryExceptions(retryAttribute.RetryExceptionTypes);
        }

        // 配置请求重试策略
        httpRequestBuilder.SetRetry(httpRetryOptions);
    }
}