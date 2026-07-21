// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpResponseResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var result = new HttpResponseResult(new HttpResponseMessage(), 100);
        Assert.NotNull(result.ResponseMessage);
        Assert.Equal(100, result.RequestDuration);
    }
}