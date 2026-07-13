// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 请求超时选项
/// </summary>
public sealed class HttpTimeoutOptions
{
    /// <summary>
    ///     超时时间
    /// </summary>
    /// <remarks>设置为 <c>null</c> 表示使用默认超时（通常为 100 秒），设置为 <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> 表示永不超时。</remarks>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    ///     超时发生时要执行的操作
    /// </summary>
    public Action? OnTimeout { get; set; }

    /// <summary>
    ///     设置超时时间
    /// </summary>
    /// <param name="timeout">超时时间，<c>null</c> 表示使用默认值</param>
    /// <returns>
    ///     <see cref="HttpTimeoutOptions" />
    /// </returns>
    public HttpTimeoutOptions SetTimeout(TimeSpan? timeout)
    {
        Timeout = timeout;

        return this;
    }

    /// <summary>
    ///     设置超时时间
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>设置为 <c>-1</c> 表示无超时</description>
    ///         </item>
    ///         <item>
    ///             <description>设置为 <c>0</c> 将立即取消请求</description>
    ///         </item>
    ///         <item>
    ///             <description>负值（除 <c>-1</c> 毫秒外）将引发 <see cref="ArgumentOutOfRangeException" /></description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <param name="milliseconds">超时时间（毫秒）</param>
    /// <returns>
    ///     <see cref="HttpTimeoutOptions" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public HttpTimeoutOptions SetTimeout(double milliseconds)
    {
        const double epsilon = 1e-9;
        var isInfinite = Math.Abs(milliseconds + 1) < epsilon;

        // 检查超时时间是否小于 0 且不等于 -1
        if (milliseconds < 0 && !isInfinite)
        {
            throw new ArgumentOutOfRangeException(nameof(milliseconds),
                "Timeout value must be greater than or equal to -1. Use -1 for infinite timeout, 0 for immediate cancellation.");
        }

        Timeout = isInfinite ? System.Threading.Timeout.InfiniteTimeSpan : TimeSpan.FromMilliseconds(milliseconds);

        return this;
    }

    /// <summary>
    ///     设置超时发生时的回调操作
    /// </summary>
    /// <param name="onTimeout">超时发生时要执行的操作</param>
    /// <returns>
    ///     <see cref="HttpTimeoutOptions" />
    /// </returns>
    public HttpTimeoutOptions SetOnTimeout(Action? onTimeout)
    {
        OnTimeout = onTimeout;

        return this;
    }
}