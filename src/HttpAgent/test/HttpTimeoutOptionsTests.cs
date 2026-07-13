// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpTimeoutOptionsTests
{
    [Fact]
    public void HttpTimeoutOptions_ReturnOK()
    {
        var options = new HttpTimeoutOptions();
        Assert.Null(options.Timeout);
        Assert.Null(options.OnTimeout);
    }

    [Fact]
    public void SetTimeout_Invalid_Parameters()
    {
        var options = new HttpTimeoutOptions();
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => options.SetTimeout(-2));
        Assert.Equal(
            "Timeout value must be greater than or equal to -1. Use -1 for infinite timeout, 0 for immediate cancellation. (Parameter 'milliseconds')",
            exception.Message);
    }

    [Fact]
    public void SetTimeout_ReturnOK()
    {
        var options = new HttpTimeoutOptions();
        options.SetTimeout(null);
        Assert.Null(options.Timeout);

        options.SetTimeout(TimeSpan.FromSeconds(1));
        Assert.Equal(TimeSpan.FromSeconds(1), options.Timeout);

        options.SetTimeout(-1);
        Assert.Equal(Timeout.InfiniteTimeSpan, options.Timeout);

        options.SetTimeout(0);
        Assert.Equal(TimeSpan.Zero, options.Timeout);

        options.SetTimeout(1000);
        Assert.Equal(TimeSpan.FromMilliseconds(1000), options.Timeout);
    }

    [Fact]
    public void SetOnTimeout_ReturnOK()
    {
        var options = new HttpTimeoutOptions();
        Assert.Null(options.OnTimeout);

        options.SetOnTimeout(null);
        Assert.Null(options.OnTimeout);

        options.SetOnTimeout(() => { });
        Assert.NotNull(options.OnTimeout);
    }
}