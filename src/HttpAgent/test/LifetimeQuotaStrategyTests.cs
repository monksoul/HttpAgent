// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class LifetimeQuotaStrategyTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var strategy = new LifetimeQuotaStrategy();
        Assert.Equal("lifetime", strategy.Name);
    }

    [Fact]
    public void TryAcquire_ReturnOK()
    {
        var quotaCounter = new HttpQuotaCounter();
        var strategy = new LifetimeQuotaStrategy();

        Assert.True(strategy.TryAcquire(quotaCounter, 100, out var quota1));
        Assert.Equal(1, quota1);
        Assert.Equal(1, quotaCounter.Count);
        Assert.Equal("lifetime", quotaCounter.WindowKey);

        Assert.True(strategy.TryAcquire(quotaCounter, 100, out var quota2));
        Assert.Equal(2, quota2);
        Assert.Equal(2, quotaCounter.Count);
        Assert.Equal("lifetime", quotaCounter.WindowKey);

        quotaCounter.WindowKey = "something_else";
        Assert.True(strategy.TryAcquire(quotaCounter, 100, out var quota3));
        Assert.Equal(3, quota3);
        Assert.Equal(3, quotaCounter.Count);
        Assert.Equal("something_else", quotaCounter.WindowKey);

        quotaCounter.WindowKey = "lifetime";
        Assert.True(strategy.TryAcquire(quotaCounter, 100, out var quota4));
        Assert.Equal(4, quota4);

        quotaCounter.Count = 100;
        Assert.False(strategy.TryAcquire(quotaCounter, 100, out var quota5));
        Assert.Equal(101, quota5);
        Assert.Equal(101, quotaCounter.Count);

        Assert.False(strategy.TryAcquire(quotaCounter, 100, out var quota6));
        Assert.Equal(102, quota6);
        Assert.Equal(102, quotaCounter.Count);
    }
}