// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     单个配额限制的配置
/// </summary>
public sealed class HttpQuotaLimit
{
    /// <summary>
    ///     <inheritdoc cref="HttpQuotaLimit" />
    /// </summary>
    public HttpQuotaLimit()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HttpQuotaLimit" />
    /// </summary>
    /// <param name="strategy">使用的配额策略名称</param>
    /// <param name="maxCount">最大允许调用次数</param>
    public HttpQuotaLimit(string? strategy, int maxCount)
    {
        Strategy = strategy;
        MaxCount = maxCount;
    }

    /// <summary>
    ///     使用的配额策略名称
    /// </summary>
    /// <remarks>必须与已注册的 <see cref="IHttpQuotaStrategy.Name" /> 匹配，否则将抛出 <see cref="InvalidOperationException" />。</remarks>
    public string? Strategy { get; set; }

    /// <summary>
    ///     最大允许调用次数
    /// </summary>
    public int MaxCount { get; set; }
}