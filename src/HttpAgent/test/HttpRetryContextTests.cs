// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRetryContextTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var context = new HttpRetryContext();
        Assert.Equal(0, context.Attempt);
        Assert.Null(context.Exception);
        Assert.Null(context.StatusCode);
        Assert.Equal(0, context.MaxRetries);
        Assert.False(context.IsExceptionRetry);
        Assert.False(context.IsStatusCodeRetry);
    }
}