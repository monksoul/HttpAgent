// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     每周配额策略
/// </summary>
internal sealed class WeeklyQuotaStrategy : IHttpQuotaStrategy
{
    /// <inheritdoc />
    public string Name => "weekly";

    /// <inheritdoc />
    public bool TryAcquire(HttpQuotaCounter quotaCounter, int maxCount, out int current)
    {
        var now = DateTime.UtcNow;

        // 计算当前周的周一日期
        var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
        var monday = now.AddDays(-diff).Date;
        var weekKey = monday.ToString("yyyy-MM-dd");

        // 如果窗口标识改变，说明进入新的一周，重置计数
        if (quotaCounter.WindowKey != weekKey)
        {
            quotaCounter.Count = 0;
            quotaCounter.WindowKey = weekKey;
        }

        // 递增计数
        quotaCounter.Count++;
        current = quotaCounter.Count;

        return current <= maxCount;
    }
}