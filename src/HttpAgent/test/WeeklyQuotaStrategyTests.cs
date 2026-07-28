// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WeeklyQuotaStrategyTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var strategy = new WeeklyQuotaStrategy();
        Assert.Equal("weekly", strategy.Name);
    }

    [Fact]
    public void TryAcquire_ReturnOK()
    {
        var now = DateTime.UtcNow;

        var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
        var thisMonday = now.AddDays(-diff).Date;
        var thisWeekKey = thisMonday.ToString("yyyy-MM-dd");

        var nextMonday = thisMonday.AddDays(7);
        var nextWeekKey = nextMonday.ToString("yyyy-MM-dd");

        var quotaCounter = new HttpQuotaCounter();
        var strategy = new WeeklyQuotaStrategy();

        Assert.True(strategy.TryAcquire(quotaCounter, 100, out var quota));
        Assert.Equal(1, quota);
        Assert.Equal(1, quotaCounter.Count);
        Assert.Equal(thisWeekKey, quotaCounter.WindowKey);

        Assert.True(strategy.TryAcquire(quotaCounter, 100, out var quota2));
        Assert.Equal(2, quota2);
        Assert.Equal(2, quotaCounter.Count);
        Assert.Equal(thisWeekKey, quotaCounter.WindowKey);

        quotaCounter.WindowKey = nextWeekKey;

        Assert.True(strategy.TryAcquire(quotaCounter, 100, out var quota3));
        Assert.Equal(1, quota3);
        Assert.Equal(1, quotaCounter.Count);
        Assert.Equal(thisWeekKey, quotaCounter.WindowKey);

        quotaCounter.Count = 100;
        Assert.False(strategy.TryAcquire(quotaCounter, 100, out var quota4));
        Assert.Equal(101, quota4);
        Assert.Equal(101, quotaCounter.Count);
    }
}