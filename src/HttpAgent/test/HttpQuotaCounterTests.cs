// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpQuotaCounterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var quotaCounter = new HttpQuotaCounter();
        Assert.Equal(0, quotaCounter.Count);
        Assert.NotNull(quotaCounter.WindowKey);
        Assert.Empty(quotaCounter.WindowKey);
    }
}