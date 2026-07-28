// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpQuotaManagerTests
{
    [Fact]
    public void New_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() => new HttpQuotaManager(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var quotaManager = new HttpQuotaManager([]);
        Assert.NotNull(quotaManager._counters);
        Assert.Empty(quotaManager._counters);
        Assert.NotNull(quotaManager._strategies);
        Assert.Empty(quotaManager._strategies);

        var quotaManager2 = new HttpQuotaManager([new DailyQuotaStrategy()]);
        Assert.NotNull(quotaManager2._strategies);
        Assert.Single(quotaManager2._strategies);
        Assert.Equal("daily", quotaManager2._strategies.Keys.ElementAt(0));
    }

    [Fact]
    public void TryIncrement_Invalid_Parameters()
    {
        var quotaManager2 = new HttpQuotaManager([new DailyQuotaStrategy()]);

        Assert.Throws<ArgumentNullException>(() => quotaManager2.TryIncrement(null, null!, null!, out _));
        Assert.Throws<ArgumentException>(() => quotaManager2.TryIncrement(null, string.Empty, null!, out _));
        Assert.Throws<ArgumentException>(() => quotaManager2.TryIncrement(null, " ", null!, out _));
        Assert.Throws<ArgumentNullException>(() => quotaManager2.TryIncrement(null, "weixin/login", null!, out _));

        var exception =
            Assert.Throws<InvalidOperationException>(() =>
                quotaManager2.TryIncrement(null, "weixin/login", new HttpQuotaLimit(), out _));
        Assert.Equal(
            "Quota limit for key 'weixin/login' has no strategy specified. Please set Strategy to a registered IHttpQuotaStrategy name (e.g., \"daily\").",
            exception.Message);

        var exception2 =
            Assert.Throws<InvalidOperationException>(() =>
                quotaManager2.TryIncrement(null, "weixin/login", new HttpQuotaLimit { Strategy = string.Empty },
                    out _));
        Assert.Equal(
            "Quota limit for key 'weixin/login' has no strategy specified. Please set Strategy to a registered IHttpQuotaStrategy name (e.g., \"daily\").",
            exception2.Message);

        var exception3 =
            Assert.Throws<InvalidOperationException>(() =>
                quotaManager2.TryIncrement(null, "weixin/login", new HttpQuotaLimit { Strategy = " " },
                    out _));
        Assert.Equal(
            "Quota limit for key 'weixin/login' has no strategy specified. Please set Strategy to a registered IHttpQuotaStrategy name (e.g., \"daily\").",
            exception3.Message);

        var exception4 =
            Assert.Throws<InvalidOperationException>(() =>
                quotaManager2.TryIncrement(null, "weixin/login", new HttpQuotaLimit { Strategy = "unknown" },
                    out _));
        Assert.Equal(
            "No quota strategy registered with name 'unknown' (required by quota key 'weixin/login'). Please use `AddDefaultQuotaStrategies()` or `AddQuotaStrategy<T>()` on the HttpRemoteBuilder to register the strategy.",
            exception4.Message);

        var exception5 =
            Assert.Throws<InvalidOperationException>(() =>
                quotaManager2.TryIncrement(null, "weixin/login", new HttpQuotaLimit { Strategy = "daily" },
                    out _));
        Assert.Equal(
            "Invalid MaxCount (0) for quota key 'weixin/login'. It must be greater than zero.",
            exception5.Message);

        var exception6 =
            Assert.Throws<InvalidOperationException>(() =>
                quotaManager2.TryIncrement(null, "weixin/login",
                    new HttpQuotaLimit { Strategy = "daily", MaxCount = -1 },
                    out _));
        Assert.Equal(
            "Invalid MaxCount (-1) for quota key 'weixin/login'. It must be greater than zero.",
            exception6.Message);
    }

    [Fact]
    public void TryIncrement_ReturnOK()
    {
        var quotaManager2 = new HttpQuotaManager([new DailyQuotaStrategy()]);

        Assert.True(quotaManager2.TryIncrement(null, "weixin/login",
            new HttpQuotaLimit { Strategy = "daily", MaxCount = 10 },
            out var current));
        Assert.Equal(1, current);
        Assert.Single(quotaManager2._counters);
        Assert.Equal(":weixin/login", quotaManager2._counters.Keys.ElementAt(0));
        Assert.Equal(1, quotaManager2._counters.Values.ElementAt(0).Count);

        Assert.True(quotaManager2.TryIncrement(null, "weixin/login",
            new HttpQuotaLimit { Strategy = "daily", MaxCount = 10 },
            out var current2));
        Assert.Equal(2, current2);
        Assert.Single(quotaManager2._counters);
        Assert.Equal(":weixin/login", quotaManager2._counters.Keys.ElementAt(0));
        Assert.Equal(2, quotaManager2._counters.Values.ElementAt(0).Count);

        Assert.True(quotaManager2.TryIncrement("weixin", "weixin/login",
            new HttpQuotaLimit { Strategy = "daily", MaxCount = 10 },
            out var current3));
        Assert.Equal(1, current3);
        Assert.Equal(2, quotaManager2._counters.Count);
        Assert.Equal("weixin:weixin/login", quotaManager2._counters.Keys.ElementAt(1));
        Assert.Equal(1, quotaManager2._counters.Values.ElementAt(1).Count);
    }
}