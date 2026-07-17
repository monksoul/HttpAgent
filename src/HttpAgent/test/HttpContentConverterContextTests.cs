// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpContentConverterContextTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var context = new HttpContentConverterContext(new HttpResponseMessage()) { RequestDuration = 10 };
        Assert.NotNull(context.ResponseMessage);
        Assert.Null(context.Converters);
        Assert.Equal(10, context.RequestDuration);
        Assert.Null(context.Factory);
    }
}