// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     每月配额策略
/// </summary>
internal sealed class MonthlyQuotaStrategy : IHttpQuotaStrategy
{
    /// <inheritdoc />
    public string Name => "monthly";

    /// <inheritdoc />
    public bool TryAcquire(HttpQuotaCounter quotaCounter, int maxCount, out int current)
    {
        // 取 UTC 当前年月作为窗口标识
        var monthKey = DateTime.UtcNow.ToString("yyyy-MM");

        // 如果窗口标识改变，说明进入新的月份，重置计数
        if (quotaCounter.WindowKey != monthKey)
        {
            quotaCounter.Count = 0;
            quotaCounter.WindowKey = monthKey;
        }

        // 递增计数
        quotaCounter.Count++;
        current = quotaCounter.Count;

        return current <= maxCount;
    }
}