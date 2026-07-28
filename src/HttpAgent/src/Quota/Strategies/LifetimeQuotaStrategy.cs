// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     生命周期总配额策略
/// </summary>
/// <remarks>不按时间重置。</remarks>
internal sealed class LifetimeQuotaStrategy : IHttpQuotaStrategy
{
    /// <inheritdoc />
    public string Name => "lifetime";

    /// <inheritdoc />
    public bool TryAcquire(HttpQuotaCounter quotaCounter, int maxCount, out int current)
    {
        // 窗口标识固定不变，确保永不重置
        if (string.IsNullOrEmpty(quotaCounter.WindowKey))
        {
            quotaCounter.WindowKey = "lifetime";
        }

        // 递增计数
        quotaCounter.Count++;
        current = quotaCounter.Count;

        return current <= maxCount;
    }
}