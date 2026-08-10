// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式重试特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public class RetryAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="RetryAttribute" />
    /// </summary>
    public RetryAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="RetryAttribute" />
    /// </summary>
    /// <param name="maxRetries">最大重试次数。0 表示不重试</param>
    public RetryAttribute(int maxRetries) => MaxRetries = maxRetries;

    /// <summary>
    ///     <inheritdoc cref="RetryAttribute" />
    /// </summary>
    /// <param name="maxRetries">最大重试次数。0 表示不重试</param>
    /// <param name="retryInterval">重试间隔基准时间（毫秒）</param>
    public RetryAttribute(int maxRetries, double retryInterval)
        : this(maxRetries) =>
        RetryInterval = retryInterval;

    /// <summary>
    ///     最大重试次数
    /// </summary>
    /// <remarks>默认值为 0，表示不重试。如果设置了 <see cref="RetryIntervals" />，此值将自动被覆盖为数组长度。</remarks>
    public int MaxRetries { get; set; }

    /// <summary>
    ///     重试间隔基准时间（毫秒）
    /// </summary>
    /// <remarks>默认值为 1000 毫秒。仅在未设置 <see cref="RetryIntervals" /> 时生效。</remarks>
    public double RetryInterval { get; set; } = 1000;

    /// <summary>
    ///     是否采用指数退避重试
    /// </summary>
    /// <remarks>
    ///     默认值为：<c>false</c>。当设置为 <c>true</c> 时，每次重试间隔 = <see cref="RetryInterval" /> * 2^(retry-1)。仅在未设置
    ///     <see cref="RetryIntervals" /> 时生效。
    /// </remarks>
    public bool UseExponentialBackoff { get; set; }

    /// <summary>
    ///     自定义重试间隔数组（毫秒）
    /// </summary>
    /// <remarks>
    ///     如果设置了此属性，则重试次数将等于数组长度，<see cref="MaxRetries" /> 和 <see cref="UseExponentialBackoff" /> 将被忽略。每次重试将按顺序使用数组中对应索引的间隔时间。
    /// </remarks>
    public double[]? RetryIntervals { get; set; }

    /// <summary>
    ///     需要重试的 HTTP 状态码集合
    /// </summary>
    /// <remarks>若为空，则仅重试因异常引发的失败。</remarks>
    public int[]? RetryStatusCodes { get; set; }

    /// <summary>
    ///     需要重试的异常类型集合
    /// </summary>
    /// <remarks>若为空，则对所有 <see cref="Exception" /> 进行重试（受 <see cref="MaxRetries" /> 限制）。</remarks>
    public Type[]? RetryExceptionTypes { get; set; }

    /// <summary>
    ///     是否无限重试，直到成功
    /// </summary>
    /// <remarks>
    ///     默认值为：<c>false</c>。当设置为 <c>true</c> 时，<see cref="MaxRetries" /> 和 <see cref="RetryIntervals" />
    ///     的长度将被忽略，一直重试直到成功或发生不可重试的异常。
    /// </remarks>
    public bool RetryIndefinitely { get; set; }
}