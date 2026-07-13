// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 请求重试上下文
/// </summary>
public sealed class HttpRetryContext
{
    /// <summary>
    ///     当前重试次数
    /// </summary>
    /// <remarks>从 1 开始。</remarks>
    public int Attempt { get; internal set; }

    /// <summary>
    ///     触发重试的异常
    /// </summary>
    public Exception? Exception { get; internal set; }

    /// <summary>
    ///     触发重试的 HTTP 状态码
    /// </summary>
    public HttpStatusCode? StatusCode { get; internal set; }

    /// <summary>
    ///     最大重试次数
    /// </summary>
    /// <remarks>-1 表示无限</remarks>
    public int MaxRetries { get; internal set; }

    /// <summary>
    ///     是否因异常触发
    /// </summary>
    public bool IsExceptionRetry => Exception is not null;

    /// <summary>
    ///     是否因状态码触发
    /// </summary>
    public bool IsStatusCodeRetry => StatusCode.HasValue;
}