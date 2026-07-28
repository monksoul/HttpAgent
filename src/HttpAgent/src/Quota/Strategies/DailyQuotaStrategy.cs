// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     每日配额策略
/// </summary>
internal sealed class DailyQuotaStrategy : IHttpQuotaStrategy
{
    /// <inheritdoc />
    public string Name => "daily";

    /// <inheritdoc />
    public bool TryAcquire(HttpQuotaCounter quotaCounter, int maxCount, out int current)
    {
        // 以 UTC 当前日期作为窗口标识
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // 如果窗口标识改变，说明进入新的一天，重置计数
        if (quotaCounter.WindowKey != today)
        {
            quotaCounter.Count = 0;
            quotaCounter.WindowKey = today;
        }

        // 递增计数
        quotaCounter.Count++;
        current = quotaCounter.Count;

        return current <= maxCount;
    }
}